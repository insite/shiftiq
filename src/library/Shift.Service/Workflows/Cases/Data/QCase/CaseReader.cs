using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseReader : IEntityReader
{
    private string DefaultSort = "CaseIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public CaseReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid issue, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.CaseIdentifier == issue && (organization == null || x.OrganizationIdentifier == organization), cancellation);

        }, cancellation);
    }

    public Task<List<CaseEntity>> CollectAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<CaseEntity> DownloadAsync(ICaseCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<CaseEntity?> RetrieveAsync(Guid issue, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.CaseIdentifier == issue, cancellation);

        }, cancellation);
    }

    public Task<List<CaseMatch>> SearchAsync(ICaseCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<CaseEntity> BuildQueryable(TableDbContext db)
    {
        return db.QCase.AsNoTracking();
    }

    private IQueryable<CaseEntity> BuildQueryable(TableDbContext db, ICaseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.CaseClosedSince.HasValue)
            query = query.Where(x => x.CaseClosed >= criteria.CaseClosedSince);

        if (criteria.CaseClosedBefore.HasValue)
            query = query.Where(x => x.CaseClosed < criteria.CaseClosedBefore);

        if (criteria.CaseOpenedSince.HasValue)
            query = query.Where(x => x.CaseOpened >= criteria.CaseOpenedSince);

        if (criteria.CaseOpenedBefore.HasValue)
            query = query.Where(x => x.CaseOpened < criteria.CaseOpenedBefore);

        if (criteria.CaseReportedSince.HasValue)
            query = query.Where(x => x.CaseReported >= criteria.CaseReportedSince);

        if (criteria.CaseReportedBefore.HasValue)
            query = query.Where(x => x.CaseReported < criteria.CaseReportedBefore);

        if (criteria.CaseStatusEffectiveSince.HasValue)
            query = query.Where(x => x.CaseStatusEffective >= criteria.CaseStatusEffectiveSince);

        if (criteria.CaseStatusEffectiveBefore.HasValue)
            query = query.Where(x => x.CaseStatusEffective < criteria.CaseStatusEffectiveBefore);

        if (criteria.LastChangeTimeSince.HasValue)
            query = query.Where(x => x.LastChangeTime >= criteria.LastChangeTimeSince);

        if (criteria.LastChangeTimeBefore.HasValue)
            query = query.Where(x => x.LastChangeTime < criteria.LastChangeTimeBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<CaseMatch>> ToMatchesAsync(IQueryable<CaseEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new CaseMatch
            {
                CaseId = entity.CaseIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}