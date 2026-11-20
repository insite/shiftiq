using System;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    /// <summary>
    /// An achievement is paid when it is a task on a program with a fee. A paid achievement reaches a
    /// learner only through the program's enrollment path, which leaves an approved request behind
    /// for billing, so the course that grants it and the pages that assign it by hand all ask here.
    /// None of these queries filter by organization: paid programs live in the shared partition-wide
    /// organization, while the courses and credentials that point at their achievements live in each
    /// organization.
    /// </summary>
    public static class PaidProgramAchievementSearch
    {
        // The refusal every hand-assignment page shows, so the admin sees one explanation wherever
        // they tried.
        public const string AssignmentRefusedMessage = "This achievement is part of a paid program. Learners receive it only by enrolling in the program.";

        public static Guid[] SelectPaidAchievementIdentifiers()
        {
            using (var db = new InternalDbContext())
            {
                return db.TTasks
                    .Where(x => x.ObjectType == ProgramTaskDefaults.AchievementObjectType && x.Program.ProgramFee > 0) 
                    .Select(x => x.ObjectIdentifier)
                    .Distinct()
                    .ToArray();
            }
        }

        public static bool IsPaidAchievement(Guid achievementIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TTasks
                    .Any(x => x.ObjectType == ProgramTaskDefaults.AchievementObjectType
                        && x.ObjectIdentifier == achievementIdentifier
                        && x.Program.ProgramFee > 0);
            }
        }

        // A course that grants a paid achievement - through its gradebook or any grade item - opens
        // only for a learner with an approved enrollment in a paid program that contains it. A course
        // that grants no paid achievement is never denied here.
        public static bool IsCourseAccessDenied(Guid courseIdentifier, Guid learnerUserIdentifier)
        {
            var achievements = SelectPaidCourseAchievementIdentifiers(courseIdentifier);
            if (achievements.Length == 0)
                return false;

            return !HasApprovedEnrollment(learnerUserIdentifier, achievements);
        }

        private static Guid[] SelectPaidCourseAchievementIdentifiers(Guid courseIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var gradebooks = db.QCourses
                    .Where(x => x.CourseIdentifier == courseIdentifier)
                    .Select(x => x.GradebookIdentifier);

                var gradebookAchievements = db.QGradebooks
                    .Where(x => gradebooks.Contains((Guid?)x.GradebookIdentifier))
                    .Select(x => x.AchievementIdentifier);

                var itemAchievements = db.QGradeItems
                    .Where(x => gradebooks.Contains((Guid?)x.GradebookIdentifier))
                    .Select(x => x.AchievementIdentifier);

                return gradebookAchievements
                    .Concat(itemAchievements)
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .Where(x => db.TTasks.Any(t => t.ObjectType == ProgramTaskDefaults.AchievementObjectType
                        && t.ObjectIdentifier == x
                        && t.Program.ProgramFee > 0))
                    .Distinct()
                    .ToArray();
            }
        }

        // True when the learner holds an approved request for a paid program that contains one of the
        // achievements, and the enrollment that approval created still exists. The request row is the
        // test rather than the enrollment alone, because a plain enrollment can be created without
        // one (the course page auto-enrols from a group assignment); only approval and Add Learners
        // to Program write an approved request. The live enrollment is required too, so a learner
        // removed from the program loses access although their billing row stays approved.
        public static bool HasApprovedEnrollment(Guid learnerUserIdentifier, Guid[] achievementIdentifiers)
        {
            if (achievementIdentifiers == null || achievementIdentifiers.Length == 0)
                return false;

            using (var db = new InternalDbContext())
            {
                var programs = db.TTasks
                    .Where(x => x.ObjectType == ProgramTaskDefaults.AchievementObjectType
                        && achievementIdentifiers.Contains(x.ObjectIdentifier)
                        && x.Program.ProgramFee > 0)
                    .Select(x => x.ProgramIdentifier);

                return db.TProgramEnrollmentRequests
                    .Where(x => x.LearnerUserIdentifier == learnerUserIdentifier
                        && x.RequestStatus == ProgramEnrollmentRequestStatus.Approved
                        && programs.Contains(x.ProgramIdentifier))
                    .Any(x => db.TProgramEnrollments.Any(e => e.EnrollmentIdentifier == x.ProgramEnrollmentIdentifier));
            }
        }
    }
}
