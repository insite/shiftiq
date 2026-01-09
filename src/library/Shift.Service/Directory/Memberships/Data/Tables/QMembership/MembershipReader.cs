using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Contract;

namespace Shift.Service.Directory;

public class MembershipReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    private string DefaultSort = "MembershipIdentifier";

    public MembershipReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid membership, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, false);

            return query.AnyAsync(x => x.MembershipIdentifier == membership, cancellation);

        }, cancellation);
    }

    public Task<List<MembershipEntity>> CollectAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<MembershipEntity> DownloadAsync(IMembershipCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<MembershipEntity?> RetrieveAsync(Guid membership, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, false);

            return query.FirstOrDefaultAsync(x => x.MembershipIdentifier == membership, cancellation);

        }, cancellation);
    }

    public Task<List<MembershipMatch>> SearchAsync(IMembershipCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter);

            return ToMatchesAsync(query, cancellation);

        }, cancellation);
    }

    /// <summary>
    /// Creates a queryable for events
    /// </summary>
    /// <remarks>
    /// If you call .Include() on the DbSet then remember to use .AsSplitQuery() so that cartesian explosion is avoided.
    /// When using split queries with Skip/Take on EF versions prior to 10, pay special attention to make your query
    /// ordering fully unique, otherwise the result set is non-deterministic.
    /// </remarks>
    private IQueryable<MembershipEntity> BuildQueryable(TableDbContext db, bool isPartitionQuery)
    {
        ValidateOrganizationContext();

        var query = db.QMembership
            .AsNoTracking()
            .Where(x => isPartitionQuery || x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<MembershipEntity> BuildQueryable(TableDbContext db, IMembershipCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        // If the user is an operator for the partition then the user is allowed to run a partition-wide query.

        var userId = _auth.GetPrincipal().User.Identifier;

        var partitionId = OrganizationIdentifiers.Global;

        var allowPartitionQuery = db.QPerson.Any(p => p.UserIdentifier == userId && p.OrganizationIdentifier == partitionId && p.IsOperator);

        var query = BuildQueryable(db, allowPartitionQuery && criteria.AccountScope == "Partition");

        // TODO: Apply criteria

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<MembershipMatch>> ToMatchesAsync(IQueryable<MembershipEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new MembershipMatch
            {
                MembershipIdentifier = entity.MembershipIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }

    private void ValidateOrganizationContext()
    {
        if (_auth.OrganizationId == Guid.Empty)
            throw new InvalidOperationException("Organization context is required");
    }
}