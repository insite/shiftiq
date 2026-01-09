using Microsoft.EntityFrameworkCore;

using Shift.Contract;

namespace Shift.Service.Progress;

public static class GradebookExtensions
{
    public static async Task<IEnumerable<GradebookMatch>> ToMatchesAsync(this IQueryable<GradebookEntity> queryable, CancellationToken cancellation = default)
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
                AchievementTitle = entity.Achievement != null ? entity.Achievement.AchievementTitle : null,
                AchievementCountGranted = entity.Achievement != null ? entity.Achievement.Credentials.Count : 0,

                IsLocked = entity.IsLocked,
            })
            .ToListAsync(cancellation);

        return matches;
    }
}