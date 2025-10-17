using Microsoft.EntityFrameworkCore;

using Shift.Common.Linq;
using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Gradebook;

public class QGradebookReader : IEntityReader
{
    private const string DefaultSort = "GradebookTitle";

    private readonly IDbContextFactory<TableDbContext> _context;
    private readonly IShiftIdentityService _identity;

    public QGradebookReader(IDbContextFactory<TableDbContext> context, IShiftIdentityService identity)
    {
        _context = context;
        _identity = identity;
    }

    public async Task<bool> AssertAsync(
        Guid gradebook,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGradebook
            .AnyAsync(x => x.GradebookIdentifier == gradebook, cancellation);
    }

    public async Task<QGradebookEntity?> RetrieveAsync(
        Guid gradebook,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await db.QGradebook
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GradebookIdentifier == gradebook, cancellation);
    }

    public async Task<int> CountAsync(
        IGradebookCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .CountAsync(cancellation);
    }

    public async Task<IEnumerable<QGradebookEntity>> CollectAsync(
        IGradebookCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .Include(x => x.Achievement)
            .Include(x => x.Event)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<QGradebookEntity>> DownloadAsync(
        IGradebookCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        return await BuildQueryable(db, criteria)
            .ToListAsync(cancellation);
    }

    public async Task<IEnumerable<GradebookMatch>> SearchAsync(
        IGradebookCriteria criteria,
        CancellationToken cancellation = default)
    {
        using var db = _context.CreateDbContext();

        var queryable = BuildQueryable(db, criteria)
            .OrderBy(criteria.Filter.Sort ?? DefaultSort)
            .ApplyPaging(criteria.Filter);

        return await ToMatchesAsync(queryable, cancellation);
    }

    private IQueryable<QGradebookEntity> BuildQueryable(
        TableDbContext db,
        IGradebookCriteria criteria)
    {
        criteria.OrganizationIdentifier = _identity.OrganizationId;

        var q = db.QGradebook.AsNoTracking().AsQueryable();

        q = q.Where(x => x.OrganizationIdentifier == criteria.OrganizationIdentifier);


        if (criteria.GradebookTitle != null)
            q = q.Where(x => x.GradebookTitle.Contains(criteria.GradebookTitle));

        if (criteria.GradebookCreatedSince != null)
            q = q.Where(x => x.GradebookCreated >= criteria.GradebookCreatedSince);

        if (criteria.GradebookCreatedBefore != null)
            q = q.Where(x => x.GradebookCreated < criteria.GradebookCreatedBefore);

        if (criteria.PeriodIdentifier != null)
            q = q.Where(x => x.PeriodIdentifier == criteria.PeriodIdentifier);


        if (criteria.AchievementIdentifier != null)
            q = q.Where(x => x.AchievementIdentifier == criteria.AchievementIdentifier);

        if (criteria.FrameworkIdentifier != null)
            q = q.Where(x => x.FrameworkIdentifier == criteria.FrameworkIdentifier);

        if (criteria.IsLocked != null)
            q = q.Where(x => x.IsLocked == criteria.IsLocked);


        if (criteria.ClassTitle != null)
            q = q.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventTitle.Contains(criteria.ClassTitle));

        if (criteria.ClassStartedSince != null)
            q = q.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventScheduledStart >= criteria.ClassStartedSince);

        if (criteria.ClassStartedBefore != null)
            q = q.Where(x => x.Event != null && x.Event.EventTitle != null && x.Event.EventScheduledStart < criteria.ClassStartedBefore);

        if (criteria.ClassIdentifier != null)
            q = q.Where(x => x.EventIdentifier == criteria.ClassIdentifier);

        if (criteria.ClassInstructorIdentifier != null)
            q = q.Where(x => x.Event != null && x.Event.Users.Any(y => y.UserIdentifier == criteria.ClassInstructorIdentifier && y.AttendeeRole == "Instructor"));


        if (criteria.GradebookType != null)
            q = q.Where(x => x.GradebookType == criteria.GradebookType);

        if (criteria.LastChangeType != null)
            q = q.Where(x => x.LastChangeType == criteria.LastChangeType);

        if (criteria.LastChangeUser != null)
            q = q.Where(x => x.LastChangeUser == criteria.LastChangeUser);

        if (criteria.Reference != null)
            q = q.Where(x => x.Reference == criteria.Reference);

        if (criteria.LastChangeSince != null)
            q = q.Where(x => x.LastChangeTime >= criteria.LastChangeSince);

        if (criteria.LastChangeBefore != null)
            q = q.Where(x => x.LastChangeTime < criteria.LastChangeBefore);

        return q;
    }

    public static async Task<IEnumerable<GradebookMatch>> ToMatchesAsync(
        IQueryable<QGradebookEntity> queryable,
        CancellationToken cancellation = default)
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
}
