using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class AttemptReader : IEntityReader
{
    private string DefaultSort = "AttemptIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public AttemptReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid attempt, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.AttemptIdentifier == attempt && (organization == null || x.OrganizationIdentifier == organization), cancellation);

        }, cancellation);
    }

    public Task<List<AttemptEntity>> CollectAsync(IAttemptCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria)
                .Include(x => x.Assessment);

            return query
                .OrderBy(criteria.Filter.Sort ?? DefaultSort)
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(IAttemptCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<AttemptEntity> DownloadAsync(IAttemptCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria)
            .Include(x => x.Assessment);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<AttemptEntity?> RetrieveAsync(Guid attempt, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db)
                .Include(x => x.Assessment);

            return query.FirstOrDefaultAsync(x => x.AttemptIdentifier == attempt, cancellation);

        }, cancellation);
    }

    public Task<List<AttemptMatch>> SearchAsync(IAttemptCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<AttemptEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.Attempt
            .AsNoTracking();

        return query;
    }

    private IQueryable<AttemptEntity> BuildQueryable(TableDbContext db, IAttemptCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId);

        if (criteria.AttemptGradedSince.HasValue)
            query = query.Where(x => x.AttemptGraded >= criteria.AttemptGradedSince);

        if (criteria.AttemptGradedBefore.HasValue)
            query = query.Where(x => x.AttemptGraded < criteria.AttemptGradedBefore);

        if (criteria.AttemptImportedSince.HasValue)
            query = query.Where(x => x.AttemptImported >= criteria.AttemptImportedSince);

        if (criteria.AttemptImportedBefore.HasValue)
            query = query.Where(x => x.AttemptImported < criteria.AttemptImportedBefore);

        if (criteria.AttemptPingedSince.HasValue)
            query = query.Where(x => x.AttemptPinged >= criteria.AttemptPingedSince);

        if (criteria.AttemptPingedBefore.HasValue)
            query = query.Where(x => x.AttemptPinged < criteria.AttemptPingedBefore);

        if (criteria.AttemptStartedSince.HasValue)
            query = query.Where(x => x.AttemptStarted >= criteria.AttemptStartedSince);

        if (criteria.AttemptStartedBefore.HasValue)
            query = query.Where(x => x.AttemptStarted < criteria.AttemptStartedBefore);

        if (criteria.AttemptStatus != null)
            query = query.Where(x => x.AttemptStatus == criteria.AttemptStatus);

        if (criteria.AttemptSubmittedSince.HasValue)
            query = query.Where(x => x.AttemptSubmitted >= criteria.AttemptSubmittedSince);

        if (criteria.AttemptSubmittedBefore.HasValue)
            query = query.Where(x => x.AttemptSubmitted < criteria.AttemptSubmittedBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<AttemptMatch>> ToMatchesAsync(IQueryable<AttemptEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new AttemptMatch
            {
                AttemptId = entity.AttemptIdentifier

            })
            .ToListAsync(cancellation);

        return matches;
    }
}