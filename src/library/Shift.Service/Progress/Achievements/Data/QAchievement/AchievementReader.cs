using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class AchievementReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private string DefaultSort = "AchievementIdentifier";

    public AchievementReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid achievement, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            if (organization != null)
                query = query.Where(x => x.OrganizationIdentifier == organization.Value);

            var exists = query.AnyAsync(x => x.AchievementIdentifier == achievement, cancellation);

            return exists;

        }, cancellation);
    }

    public Task<List<AchievementEntity>> CollectAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<AchievementEntity> DownloadAsync(IAchievementCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<AchievementEntity?> RetrieveAsync(Guid achievement, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.AchievementIdentifier == achievement, cancellation);

        }, cancellation);
    }

    public Task<List<AchievementMatch>> SearchAsync(IAchievementCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<AchievementEntity> BuildQueryable(TableDbContext db)
    {
        var query = db.QAchievement
            .AsNoTracking();

        return query;
    }

    private IQueryable<AchievementEntity> BuildQueryable(TableDbContext db, IAchievementCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.AchievementTitle != null)
            query = query.Where(x => x.AchievementTitle.Contains(criteria.AchievementTitle));

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<AchievementMatch>> ToMatchesAsync(IQueryable<AchievementEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new AchievementMatch
            {
                AchievementId = entity.AchievementIdentifier,
                AchievementTitle = entity.AchievementTitle
            })
            .ToListAsync(cancellation);

        return matches;
    }
}
