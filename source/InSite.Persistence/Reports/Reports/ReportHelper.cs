using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public class CommonReportHelper
    {
        public class ProgramAchievement
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string GroupType { get; set; }
            public string GroupName { get; set; }
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementType { get; set; }
            public string Status { get; set; }
            public Guid? ProgramIdentifier { get; set; }
            public string ProgramName { get; set; }
            public string ProgramProgress { get; set; }
            public int ProgramCount { get; set; }
            public int? TimeTakenDays { get; set; }
        }

        public class ProgramEnrollment
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string GroupType { get; set; }
            public string GroupName { get; set; }
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public string Status { get; set; }
            public Guid ProgramIdentifier { get; set; }
            public string ProgramName { get; set; }
            public DateTimeOffset? ProgramStartDate { get; set; }
            public DateTimeOffset? ProgramCompletedDate { get; set; }
            public int? TimeTakenDays { get; set; }
        }

        public static IEnumerable<ProgramAchievement> SelectProgramAchievements(
            Guid organizationIdentifier,
            string groupType,
            Guid[] groups,
            Guid[] programs,
            Guid[] achievements,
            Guid[] learners,
            string credentialStatus)
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@GroupType",              string.IsNullOrEmpty(groupType) ? (object)DBNull.Value : groupType),
                new SqlParameter("@Groups",                 groups.IsNotEmpty() ? string.Join(",", groups) : (object)DBNull.Value),
                new SqlParameter("@Programs",               programs.IsNotEmpty() ? string.Join(",", programs) : (object)DBNull.Value),
                new SqlParameter("@Achievements",           string.Join(",", achievements)),
                new SqlParameter("@Learners",               learners.IsNotEmpty() ? string.Join(",", learners) : (object)DBNull.Value),
                new SqlParameter("@CredentialStatus",       string.IsNullOrEmpty(credentialStatus) ? (object)DBNull.Value : credentialStatus),
                new SqlParameter("@OrganizationIdentifier", organizationIdentifier),
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5; // 5 minutes

                return db.Database
                    .SqlQuery<ProgramAchievement>(
                        "EXEC reports.SelectProgramAchievements" +
                        "  @GroupType" +
                        ", @Groups" +
                        ", @Programs" +
                        ", @Achievements" +
                        ", @Learners" +
                        ", @CredentialStatus" +
                        ", @OrganizationIdentifier"
                        , sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<ProgramEnrollment> SelectProgramEnrollments(
            Guid organizationIdentifier,
            string groupType,
            Guid[] groups,
            Guid[] programs,
            Guid[] achievements,
            Guid[] learners,
            string credentialStatus)
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@GroupType",              groupType.IsNotEmpty() ? groupType : (object)DBNull.Value),
                new SqlParameter("@Groups",                 groups.IsNotEmpty() ? string.Join(",", groups) : (object)DBNull.Value),
                new SqlParameter("@Programs",               programs.IsNotEmpty() ? string.Join(",", programs) : (object)DBNull.Value),
                new SqlParameter("@Achievements",           achievements.IsNotEmpty() ? string.Join(",", achievements) : (object)DBNull.Value),
                new SqlParameter("@Learners",               learners.IsNotEmpty() ? string.Join(",", learners) : (object)DBNull.Value),
                new SqlParameter("@CredentialStatus",       credentialStatus.IsNotEmpty() ? credentialStatus : (object)DBNull.Value),
                new SqlParameter("@OrganizationIdentifier", organizationIdentifier),
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5; // 5 minutes

                return db.Database
                    .SqlQuery<ProgramEnrollment>(
                        "EXEC reports.SelectProgramEnrollments" +
                        "  @GroupType" +
                        ", @Groups" +
                        ", @Programs" +
                        ", @Achievements" +
                        ", @Learners" +
                        ", @CredentialStatus" +
                        ", @OrganizationIdentifier"
                        , sqlParameters)
                    .ToList();
            }
        }
    }
}
