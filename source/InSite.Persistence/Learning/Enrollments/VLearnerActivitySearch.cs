using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public class VLearnerActivitySearch
    {
        private class VLearnerActivityReadHelper : ReadHelper<VLearnerActivity>
        {
            public static readonly VLearnerActivityReadHelper Instance = new VLearnerActivityReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VLearnerActivity>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.VLearnerActivities.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static int Count(VLearnerActivityFilter filter) =>
            VLearnerActivityReadHelper.Instance.Count(
                (IQueryable<VLearnerActivity> query) => query.Filter(filter));

        public static IList<T> Bind<T>(Expression<Func<VLearnerActivity, T>> binder, VLearnerActivityFilter filter)
        {
            return VLearnerActivityReadHelper.Instance.Bind(
                (IQueryable<VLearnerActivity> query) => query.Select(binder),
                (IQueryable<VLearnerActivity> query) => query.Filter(filter),
                filter.Paging, filter.OrderBy, null);
        }

        public static IEnumerable<string> GetComboBoxItems(Guid organization, string column)
        {
            string query = $@"
SELECT {column} FROM records.VLearnerActivity WHERE OrganizationIdentifier = '{organization}' AND {column} IS NOT NULL GROUP BY {column} ORDER BY {column}
";
            using (var db = new InternalDbContext())
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
            using (var db = new InternalDbContext())
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
                    x.LearnerRole,
                    x.PersonCode,

                    x.LearnerPhone,
                    x.LearnerBirthdate,
                    x.LearnerOccupation,
                    x.LearnerConsent,

                    x.ImmigrationNumber,
                    x.ImmigrationArrival,
                    x.ImmigrationStatus,
                    x.ImmigrationDestination,

                    x.ReferrerName,
                    x.ReferrerOther,

                    x.OrganizationIdentifier,
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
                    LearnerRole = x.Key.LearnerRole,
                    PersonCode = x.Key.PersonCode,

                    LearnerPhone = x.Key.LearnerPhone,
                    LearnerBirthdate = x.Key.LearnerBirthdate,
                    LearnerOccupation = x.Key.LearnerOccupation,
                    LearnerConsent = x.Key.LearnerConsent,

                    ImmigrationNumber = x.Key.ImmigrationNumber,
                    ImmigrationArrival = x.Key.ImmigrationArrival,
                    ImmigrationStatus = x.Key.ImmigrationStatus,
                    ImmigrationDestination = x.Key.ImmigrationDestination,

                    ReferrerName = x.Key.ReferrerName,
                    ReferrerOther = x.Key.ReferrerOther,

                    OrganizationIdentifier = x.Key.OrganizationIdentifier,

                    EnrollmentStarted = x.Min(y => y.EnrollmentStarted),
                    SessionStartedFirst = x.Min(y => y.SessionStartedFirst),
                    SessionStartedLast = x.Max(y => y.SessionStartedLast),
                    SessionCount = x.Max(y => y.SessionCount),
                    SessionMinutes = x.Max(y => y.SessionMinutes),

                    ProgramName = string.Join("; ", x.Select(y => y.ProgramName).Distinct()),
                    GradebookName = string.Join("; ", x.Select(y => y.GradebookName).Distinct()),
                    EnrollmentStatus = string.Join("; ", x.Select(y => y.EnrollmentStatus).Distinct()),

                    CertificateGranted = x.Max(y => y.CertificateGranted),
                })
                .ToList();

            return summaries;
        }
    }
}
