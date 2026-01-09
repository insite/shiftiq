using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class VLearnerActivitySearch
    {
        public static int Count(VLearnerActivityFilter filter)
        {
            using (var db = CreateContext())
                return db.VLearnerActivities.Filter(filter, db).Count();
        }

        public static IList<T> Bind<T>(Expression<Func<VLearnerActivity, T>> binder, VLearnerActivityFilter filter)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Filter(filter, db)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .Select(binder)
                    .ToArray();
            }
        }

        public static IList<VLearnerActivity> Select(VLearnerActivityFilter filter)
        {
            using (var db = CreateContext())
            {
                return db.VLearnerActivities
                    .Filter(filter, db)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .ToArray();
            }
        }

        public static IList<VProgramEnrollment> SelectProgramEnrollments(VLearnerActivityFilter filter)
        {
            using (var db = CreateContext())
            {
                var query = db.VLearnerActivities.AsQueryable().Filter(filter, db);

                return db.VProgramEnrollments
                    .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier
                             && query.Select(y => y.LearnerIdentifier).Contains(x.UserIdentifier))
                    .ToArray();
            }
        }

        public static IEnumerable<string> GetComboBoxItems(Guid organization, string column)
        {
            string query = $@"
SELECT {column} FROM records.VLearnerActivity WHERE OrganizationIdentifier = '{organization}' AND {column} IS NOT NULL GROUP BY {column} ORDER BY {column}
";
            using (var db = CreateContext())
            {
                var list = db.Database.SqlQuery<string>(query).ToList();
                if (list.Count > 0)
                    return list;
                return new string[] { string.Empty };
            }
        }

        public static List<VLearnerProgramCount> GetLearnersEnrolledInMultiplePrograms()
        {
            string query = $@"
WITH cte
AS (SELECT DISTINCT
           LearnerName
         , ProgramName
    FROM records.VLearnerActivity)
SELECT cte.LearnerName
     , COUNT(*) AS ProgramCount
     , STRING_AGG(ProgramName, ', ') WITHIN GROUP (ORDER BY ProgramName) AS ProgramNames
FROM cte
GROUP BY cte.LearnerName
HAVING COUNT(*) > 1
ORDER BY cte.LearnerName
";
            using (var db = CreateContext())
                return db.Database.SqlQuery<VLearnerProgramCount>(query).ToList();
        }

        public static IList<VLearnerActivitySummary> Summarize(IEnumerable<VLearnerActivity> list)
        {
            var summaries = list
                .GroupBy(x => new
                {
                    x.LearnerIdentifier,
                    x.LearnerName,
                    x.LearnerNameLast,
                    x.LearnerNameFirst,
                    x.LearnerEmail,
                    x.LearnerGender,
                    x.LearnerCreated,
                    x.LearnerCitizenship,
                    x.PersonCode,

                    x.LearnerPhone,
                    x.LearnerBirthdate,
                    x.LearnerOccupation,

                    x.EmployerGroupName,
                    x.MembershipStatusItemName,
                })
                .Select(x => new VLearnerActivitySummary
                {
                    LearnerIdentifier = x.Key.LearnerIdentifier,
                    LearnerName = x.Key.LearnerName,
                    LearnerNameLast = x.Key.LearnerNameLast,
                    LearnerNameFirst = x.Key.LearnerNameFirst,
                    LearnerEmail = x.Key.LearnerEmail,
                    LearnerGender = x.Key.LearnerGender,
                    LearnerCreated = x.Key.LearnerCreated,
                    LearnerCitizenship = x.Key.LearnerCitizenship,
                    PersonCode = x.Key.PersonCode,

                    LearnerPhone = x.Key.LearnerPhone,
                    LearnerBirthdate = x.Key.LearnerBirthdate,
                    LearnerOccupation = x.Key.LearnerOccupation,

                    MembershipStatus = x.Key.MembershipStatusItemName,
                    EmployerName = x.Key.EmployerGroupName,

                    EnrollmentStarted = x.Min(y => y.EnrollmentStarted),
                    SessionStartedFirst = x.Min(y => y.SessionStartedFirst),
                    SessionStartedLast = x.Max(y => y.SessionStartedLast),
                    SessionCount = x.Max(y => y.SessionCount),
                    SessionMinutes = x.Max(y => y.SessionMinutes),

                    GradebookName = string.Join("; ", x.Select(y => y.GradebookName).Distinct()),
                    EnrollmentStatus = string.Join("; ", x.Select(y => y.EnrollmentStatus).Distinct()),

                    AchievementGranted = x.Max(y => y.CertificateGranted),
                })
                .ToList();

            return summaries;
        }

        private static InternalDbContext CreateContext() => new InternalDbContext(false, false);
    }
}
