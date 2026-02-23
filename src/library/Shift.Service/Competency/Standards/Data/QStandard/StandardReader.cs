using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Competency;

public class StandardReader : IEntityReader
{
    private string DefaultSort = "StandardIdentifier";

    private readonly IDbContextFactory<TableDbContext> _context;

    public StandardReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid standard, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.StandardIdentifier == standard && (organization == null || organization == x.OrganizationIdentifier), cancellation);

        }, cancellation);
    }

    public Task<List<StandardEntity>> CollectAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<StandardEntity> DownloadAsync(IStandardCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<StandardEntity?> RetrieveAsync(Guid standard, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.StandardIdentifier == standard, cancellation);

        }, cancellation);
    }

    public Task<List<StandardMatch>> SearchAsync(IStandardCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<StandardEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QStandard
            .AsNoTracking();

        return query;
    }

    private IQueryable<StandardEntity> BuildQueryable(TableDbContext db, IStandardCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (!string.IsNullOrEmpty(criteria.ContentTitle))
            query = query.Where(x => x.ContentTitle!.Contains(criteria.ContentTitle));

        if (!string.IsNullOrEmpty(criteria.StandardType))
            query = query.Where(x => x.StandardType == criteria.StandardType);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<StandardMatch>> ToMatchesAsync(IQueryable<StandardEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new StandardMatch
            {
                Code = entity.Code,
                Id = entity.StandardIdentifier,
                Name = entity.ContentName,
                Title = entity.ContentTitle,
                Type = entity.StandardType
            })
            .ToListAsync(cancellation);

        return matches;
    }
}