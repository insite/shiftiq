using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class PeriodReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    private string DefaultSort = "PeriodName, PeriodIdentifier";

    public PeriodReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid period, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.PeriodIdentifier == period, cancellation);

        }, cancellation);
    }

    public Task<List<PeriodEntity>> CollectAsync(IPeriodCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IPeriodCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<PeriodEntity> DownloadAsync(IPeriodCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<PeriodEntity?> RetrieveAsync(Guid period, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.PeriodIdentifier == period, cancellation);

        }, cancellation);
    }

    public Task<List<PeriodMatch>> SearchAsync(IPeriodCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<PeriodEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.QPeriod
            .AsNoTracking()
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<PeriodEntity> BuildQueryable(TableDbContext db, IPeriodCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.Name != null)
            query = query.Where(x => x.PeriodName.Contains(criteria.Name));

        if (criteria.StartSince != null)
            query = query.Where(x => x.PeriodStart >= criteria.StartSince);

        if (criteria.StartBefore != null)
            query = query.Where(x => x.PeriodStart < criteria.StartBefore);

        if (criteria.EndSince != null)
            query = query.Where(x => x.PeriodEnd >= criteria.EndSince);

        if (criteria.EndBefore != null)
            query = query.Where(x => x.PeriodEnd < criteria.EndBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<PeriodMatch>> ToMatchesAsync(IQueryable<PeriodEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new PeriodMatch
            {
                Id = entity.PeriodIdentifier,
                Name = entity.PeriodName
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
