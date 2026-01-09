using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Contract;

namespace Shift.Service.Progress;

public class GradebookReader : IEntityReader
{
    private readonly IDbContextFactory<TableDbContext> _context;

    private readonly IShiftIdentityService _auth;

    private string DefaultSort = "GradebookTitle, GradebookIdentifier";

    public GradebookReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService auth)
    {
        _context = context;
        _auth = auth;
    }

    public Task<bool> AssertAsync(Guid gradebook, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.AnyAsync(x => x.GradebookIdentifier == gradebook, cancellation);

        }, cancellation);
    }

    public TimeZoneInfo GetTimeZone()
    {
        return _auth.GetTimeZone();
    }

    public Task<List<GradebookEntity>> CollectAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
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

    public Task<int> CountAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db, criteria);

            return query.CountAsync(cancellation);

        }, cancellation);
    }

    public async IAsyncEnumerable<GradebookEntity> DownloadAsync(IGradebookCriteria criteria, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var query = BuildQueryable(db, criteria);

        await foreach (var entity in query.AsAsyncEnumerable().WithCancellation(cancellation))
        {
            yield return entity;
        }
    }

    public Task<GradebookEntity?> RetrieveAsync(Guid gradebook, CancellationToken cancellation = default)
    {
        return ExecuteAsync(db =>
        {
            var query = BuildQueryable(db);

            return query.FirstOrDefaultAsync(x => x.GradebookIdentifier == gradebook, cancellation);

        }, cancellation);
    }

    public Task<List<GradebookMatch>> SearchAsync(IGradebookCriteria criteria, CancellationToken cancellation = default)
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
    private IQueryable<GradebookEntity> BuildQueryable(TableDbContext db)
    {
        ValidateOrganizationContext();

        var query = db.QGradebook
            .AsNoTracking()
            .Include(x => x.Achievement)
            .Include(x => x.Event)
            .Where(x => x.OrganizationIdentifier == _auth.OrganizationId);

        return query;
    }

    private IQueryable<GradebookEntity> BuildQueryable(TableDbContext db, IGradebookCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria?.Filter, nameof(criteria.Filter));

        var query = BuildQueryable(db);

        if (criteria.GradebookTitle != null)
            query = query.Where(x => x.GradebookTitle.Contains(criteria.GradebookTitle));

        if (criteria.GradebookCreatedSince != null)
            query = query.Where(x => x.GradebookCreated >= criteria.GradebookCreatedSince);

        if (criteria.GradebookCreatedBefore != null)
            query = query.Where(x => x.GradebookCreated < criteria.GradebookCreatedBefore);

        if (criteria.PeriodIdentifier != null)
            query = query.Where(x => x.PeriodIdentifier == criteria.PeriodIdentifier);


        if (criteria.AchievementIdentifier != null)
            query = query.Where(x => x.AchievementIdentifier == criteria.AchievementIdentifier);

        if (criteria.FrameworkIdentifier != null)
            query = query.Where(x => x.FrameworkIdentifier == criteria.FrameworkIdentifier);

        if (criteria.IsLocked != null)
            query = query.Where(x => x.IsLocked == criteria.IsLocked);


        if (criteria.ClassTitle != null)
            query = query.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventTitle.Contains(criteria.ClassTitle));

        if (criteria.ClassStartedSince != null)
            query = query.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventScheduledStart >= criteria.ClassStartedSince);

        if (criteria.ClassStartedBefore != null)
            query = query.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventScheduledStart < criteria.ClassStartedBefore);

        if (criteria.ClassIdentifier != null)
            query = query.Where(x => x.EventIdentifier == criteria.ClassIdentifier);

        if (criteria.ClassInstructorIdentifier != null)
            query = query.Where(x => x.Event != null && x.Event.Users.Any(y => y.UserIdentifier == criteria.ClassInstructorIdentifier && y.AttendeeRole == "Instructor"));


        if (criteria.GradebookType != null)
            query = query.Where(x => x.GradebookType == criteria.GradebookType);

        if (criteria.LastChangeType != null)
            query = query.Where(x => x.LastChangeType == criteria.LastChangeType);

        if (criteria.LastChangeUser != null)
            query = query.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        if (criteria.Reference != null)
            query = query.Where(x => x.Reference == criteria.Reference);

        if (criteria.LastChangeSince != null)
            query = query.Where(x => x.LastChangeTime >= criteria.LastChangeSince);

        if (criteria.LastChangeBefore != null)
            query = query.Where(x => x.LastChangeTime < criteria.LastChangeBefore);

        return query;
    }

    private async Task<T> ExecuteAsync<T>(Func<TableDbContext, Task<T>> query, CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await query(db);
    }

    public static async Task<List<GradebookMatch>> ToMatchesAsync(IQueryable<GradebookEntity> queryable, CancellationToken cancellation = default)
    {
        var matches = await queryable
            .Select(entity => new GradebookMatch
            {
                GradebookIdentifier = entity.GradebookIdentifier,
                GradebookTitle = entity.GradebookTitle,
                GradebookCreated = entity.GradebookCreated,
                GradebookEnrollmentCount = entity.Enrollments.Count,

                ClassIdentifier = entity.EventIdentifier,
                ClassTitle = entity.Event != null ? entity.Event.EventTitle : null,
                ClassStarted = entity.Event != null ? entity.Event.EventScheduledStart : null,
                ClassEnded = entity.Event != null ? entity.Event.EventScheduledEnd : null,

                AchievementIdentifier = entity.AchievementIdentifier,
                AchievementTitle = entity.Achievement != null ? entity.Achievement.AchievementTitle : null
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
