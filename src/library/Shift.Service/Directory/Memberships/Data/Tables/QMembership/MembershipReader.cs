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

    private string DefaultSort = "MembershipIdentifier";

    public MembershipReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid membership, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = db.QMembership.AsNoTracking();

            return query.AnyAsync(x => x.MembershipIdentifier == membership && (organization == null || organization == x.OrganizationIdentifier), cancellation);

        }, cancellation);
    }

    public Task<List<MembershipEntity>> CollectAsync(IMembershipCriteria criteria, Guid currentUserId, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria, currentUserId);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IMembershipCriteria criteria, Guid currentUserId, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria, currentUserId);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<MembershipEntity> DownloadAsync(IMembershipCriteria criteria, Guid currentUserId, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria, currentUserId);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<MembershipEntity?> RetrieveAsync(Guid membership, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = db.QMembership.AsNoTracking();

            return query.FirstOrDefaultAsync(x => x.MembershipIdentifier == membership, cancellation);

        }, cancellation);
    }

    public Task<List<MembershipMatch>> SearchAsync(IMembershipCriteria criteria, Guid currentUserId, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria, currentUserId);

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
        var query = db.QMembership
            .AsNoTracking();

        return query;
    }

    private IQueryable<MembershipEntity> BuildQueryable(TableDbContext db, IMembershipCriteria criteria, Guid currentUserId)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        // If the user is an operator for the partition then the user is allowed to run a partition-wide query.

        var partitionId = OrganizationIdentifiers.Global;

        var allowPartitionQuery = db.QPerson.Any(p => p.UserIdentifier == currentUserId && p.OrganizationIdentifier == partitionId && p.IsOperator);

        var query = BuildQueryable(db, allowPartitionQuery && criteria.AccountScope == "Partition");

        // TODO: Apply criteria

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

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
                MembershipId = entity.MembershipIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}