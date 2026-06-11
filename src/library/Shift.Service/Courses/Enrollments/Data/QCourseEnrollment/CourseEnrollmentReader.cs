using Microsoft.EntityFrameworkCore;

using Shift.Common;

namespace Shift.Service.Learning;

public class CourseEnrollmentReader(IDbContextFactory<TableDbContext> context) : IEntityReader
{
    public async Task<int> CountStartedEnrollmentsAsync(Guid organizationId)
    {
        using var db = context.CreateDbContext();

        return await db.Course
            .Where(x => x.OrganizationIdentifier == organizationId)
            .Join(db.QGradebookEnrollment.Where(x => x.EnrollmentStarted != null),
                c => c.GradebookIdentifier,
                e => e.GradebookIdentifier,
                (c, e) => e
            )
            .CountAsync();
    }

    public async Task<int> CountCompletedEnrollmentsAsync(Guid organizationId)
    {
        using var db = context.CreateDbContext();

        return await db.Course
            .Where(x => x.OrganizationIdentifier == organizationId)
            .Join(db.CourseEnrollment.Where(x => x.CourseCompleted != null),
                c => c.CourseIdentifier,
                ce => ce.CourseIdentifier,
                (c, ce) => new
                {
                    GradebookIdentifier = (Guid)c.GradebookIdentifier!,
                    LearnerIdentifier = ce.LearnerUserIdentifier,

                }
            )
            .Join(db.QGradebookEnrollment,
                ce => new { ce.GradebookIdentifier, ce.LearnerIdentifier },
                e => new { e.GradebookIdentifier, e.LearnerIdentifier },
                (c, e) => e
            )
            .CountAsync();
    }
}