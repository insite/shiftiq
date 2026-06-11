using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Learning;

public class CourseReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    public CourseReader(IDbContextFactory<TableDbContext> context)
    {
        _context = context;
    }

    public Task<bool> AssertAsync(Guid course, Guid? organization, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db);

            return query.AnyAsync(x => x.CourseIdentifier == course
                    && (organization == null || organization == x.OrganizationIdentifier),
                cancellation);

        }, cancellation);
    }

    public Task<List<CourseEntity>> CollectAsync(ICourseCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db, criteria);

            return query
                .OrderBy(criteria.Filter.Sort ?? "CourseIdentifier")
                .ApplyPaging(criteria.Filter)
                .ToListAsync(cancellation);

        }, cancellation);
    }

    public Task<int> CountAsync(ICourseCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<CourseEntity> DownloadAsync(ICourseCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildEntityQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<CourseEntity?> RetrieveAsync(Guid course, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildEntityQueryable(db);

            return query.FirstOrDefaultAsync(x => x.CourseIdentifier == course, cancellation);

        }, cancellation);
    }

    public Task<List<CourseMatch>> SearchAsync(ICourseCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildMatchQueryable(db, criteria);

            query = query
                .OrderBy(criteria.Filter.Sort ?? "CourseId")
                .ApplyPaging(criteria.Filter);

            return query.ToListAsync(cancellation);

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
    private IQueryable<CourseEntity> BuildEntityQueryable(TableDbContext db)
    {
        var query = db.Course
            .AsNoTracking();

        return query;
    }

    private IQueryable<CourseEntity> BuildEntityQueryable(TableDbContext db, ICourseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildEntityQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationIdentifier == criteria.OrganizationId.Value);

        if (criteria.ClosedSince.HasValue)
            query = query.Where(x => x.Closed >= criteria.ClosedSince);

        if (criteria.ClosedBefore.HasValue)
            query = query.Where(x => x.Closed < criteria.ClosedBefore);

        if (criteria.CreatedSince.HasValue)
            query = query.Where(x => x.Created >= criteria.CreatedSince);

        if (criteria.CreatedBefore.HasValue)
            query = query.Where(x => x.Created < criteria.CreatedBefore);

        if (criteria.ModifiedSince.HasValue)
            query = query.Where(x => x.Modified >= criteria.ModifiedSince);

        if (criteria.ModifiedBefore.HasValue)
            query = query.Where(x => x.Modified < criteria.ModifiedBefore);

        if (criteria.IsPublished != null)
        {
            var subQuery = db.QPage.AsQueryable()
                .Where(p => p.ObjectType == "Course" && p.ContentControl == "Course" && p.IsHidden == false)
                .Select(p => p.ObjectIdentifier);

            if (criteria.IsPublished.Value)
                query = query.Where(x => subQuery.Contains(x.CourseIdentifier));
            else
                query = query.Where(x => !subQuery.Contains(x.CourseIdentifier));
        }

        return query;
    }

    private IQueryable<CourseMatch> BuildMatchQueryable(TableDbContext db)
    {
        var query = db.CourseMatch
            .AsNoTracking();

        return query;
    }

    private IQueryable<CourseMatch> BuildMatchQueryable(TableDbContext db, ICourseCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildMatchQueryable(db);

        if (criteria.OrganizationId != null)
            query = query.Where(x => x.OrganizationId == criteria.OrganizationId.Value);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }
}