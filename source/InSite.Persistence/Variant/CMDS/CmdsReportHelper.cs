using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class CmdsReportHelper
    {
        #region Classes

        public class DepartmentCompetency
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public string CompanyName { get; set; }
            public string Number { get; set; }
            public string Summary { get; set; }
            public string ProfileNumber { get; set; }
            public string ProfileTitle { get; set; }
            public bool IsTimeSensitive { get; set; }
            public int? Lifetime { get; set; }
            public string ValidForText { get; set; }
            public string PriorityText { get; set; }
            public string LevelName { get; set; }
        }

        public class BillableUser
        {
            public string BillingClassification { get; set; }
            public string CompanyName { get; set; }
            public int SharedCompanyCount { get; set; }
            public string Category { get; set; }
            public int UserCount { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Amount { get; set; }
        }

        public class ComplianceSummary
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public Guid? PrimaryProfileIdentifier { get; set; }
            public string PrimaryProfileNumber { get; set; }
            public string PrimaryProfileTitle { get; set; }
            public int Sequence { get; set; }
            public string Heading { get; set; }
            public int Required { get; set; }
            public int Satisfied { get; set; }
            public decimal Score { get; set; }
            public int? Expired { get; set; }
            public int? NotCompleted { get; set; }
            public int? NotApplicable { get; set; }
            public int? NeedsTraining { get; set; }
            public int? SelfAssessed { get; set; }
            public int? Submitted { get; set; }
            public int? Validated { get; set; }
        }

        [Serializable]
        public class TrainingRequirementPerCompetency
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string Number { get; set; }
            public string Summary { get; set; }
        }

        public class EmployeeComplianceHistory
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public Guid? PrimaryProfileIdentifier { get; set; }
            public string PrimaryProfileNumber { get; set; }
            public string PrimaryProfileTitle { get; set; }
            public int Sequence { get; set; }
            public string Heading { get; set; }
            public int Required { get; set; }
            public int Satisfied { get; set; }
            public decimal Score { get; set; }
            public int? Expired { get; set; }
            public int? NotCompleted { get; set; }
            public int? NotApplicable { get; set; }
            public int? NeedsTraining { get; set; }
            public int? SelfAssessed { get; set; }
            public int? Submitted { get; set; }
            public int? Validated { get; set; }
            public DateTime? SnapshotDate1 { get; set; }
            public decimal? Score1 { get; set; }
            public DateTime? SnapshotDate2 { get; set; }
            public decimal? Score2 { get; set; }
            public DateTime? SnapshotDate3 { get; set; }
            public decimal? Score3 { get; set; }
        }

        public class EmployeeComplianceHistoryChart
        {
            public Guid DepartmentIdentifier { get; set; }
            public int Sequence { get; set; }
            public string Heading { get; set; }
            public DateTime? SnapshotDate { get; set; }
            public decimal? Score { get; set; }
        }

        [Serializable]
        public class EmployeeComplianceDepartment
        {
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public int UserCount { get; set; }
        }

        public class CollegeCertificate
        {
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid PersonId { get; set; }
            public string FullName { get; set; }
            public Guid ProfileStandardIdentifier { get; set; }
            public string ProfileNumber { get; set; }
            public string ProfileTitle { get; set; }
            public decimal CoreHoursCompleted { get; set; }
            public decimal NonCoreHoursCompleted { get; set; }
            public decimal CoreHoursTotal { get; set; }
            public decimal NonCoreHoursTotal { get; set; }
            public decimal CertificationHoursPercentCore { get; set; }
            public decimal CertificationHoursPercentNonCore { get; set; }
            public DateTimeOffset? DateRequested { get; set; }
            public DateTimeOffset? DateGranted { get; set; }
            public DateTimeOffset? DateSubmitted { get; set; }
            public string InstitutionName { get; set; }
            public decimal CoreHoursRequired { get; set; }
            public decimal NonCoreHoursRequired { get; set; }
            public string CertificateType { get; set; }
        }

        public class CompanySummary
        {
            public class Department
            {
                public Guid DepartmentIdentifier { get; set; }
                public string DepartmentName { get; set; }
                public int UserCount { get; set; }
            }

            public class User
            {
                public string FullName { get; set; }
                public DateTimeOffset? LastAuthenticated { get; set; }
                public string PersonDepartments { get; set; }
            }

            public class Role
            {
                public int Sequence { get; set; }
                public string RoleName { get; set; }
                public int UserCount { get; set; }
            }

            public Guid OrganizationIdentifier { get; set; }
            public string Name { get; set; }
            public int UserCount { get; set; }

            public IEnumerable<Department> Departments { get; set; }
            // public IEnumerable<User> Users { get; set; }
            public IEnumerable<Role> Roles { get; set; }
        }

        public class CompetencyPerProfile
        {
            public string ProfileNumber { get; set; }
            public string ProfileTitle { get; set; }
            public string CompetencyNumber { get; set; }
            public string CompetencySummary { get; set; }
            public string CompetencyKnowledge { get; set; }
            public string CompetencySkills { get; set; }
            public decimal? ProgramHours { get; set; }

            public string CompetencyDescription
            {
                get
                {
                    var md = new StringBuilder();

                    md.AppendLine($"**{CompetencySummary}**");
                    md.AppendLine();

                    if (CompetencyKnowledge.IsNotEmpty())
                    {
                        md.AppendLine("**Knowledge:**");
                        md.AppendLine();
                        md.AppendLine(Cleanup(CompetencyKnowledge));
                        md.AppendLine();
                    }

                    if (CompetencySkills.IsNotEmpty())
                    {
                        md.AppendLine("**Skills:**");
                        md.AppendLine();
                        md.AppendLine(Cleanup(CompetencySkills));
                        md.AppendLine();
                    }

                    return Markdown.ToHtml(md.ToString());
                }
            }

            private string Cleanup(string text)
                => text != null ? StringHelper.Remove(text, new[] { "# Knowledge", "# Skills" }) : null;
        }

        public class CompetencyStatusHistoryChart
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public string EmployeeName { get; set; }
            public DateTime SnapshotDate { get; set; }
            public string DatePeriod { get; set; }
            public int CompetencyCountRequired { get; set; }
            public int CompetencyCountExpired { get; set; }
            public int CompetencyCountNotCompleted { get; set; }
            public int CompetencyCountNotApplicable { get; set; }
            public int CompetencyCountNeedsTraining { get; set; }
            public int CompetencyCountSelfAssessed { get; set; }
            public int CompetencyCountSubmitted { get; set; }
            public int CompetencyCountValidated { get; set; }
            public decimal CompetencyPercentExpired { get; set; }
            public decimal CompetencyPercentNotCompleted { get; set; }
            public decimal CompetencyPercentNotApplicable { get; set; }
            public decimal CompetencyPercentNeedsTraining { get; set; }
            public decimal CompetencyPercentSelfAssessed { get; set; }
            public decimal CompetencyPercentSubmitted { get; set; }
            public decimal CompetencyPercentValidated { get; set; }
        }

        public class DepartmentProfileSummary
        {
            public class Employee
            {
                public Guid ProfileStandardIdentifier { get; set; }
                public Guid UserIdentifier { get; set; }
                public string PersonFullName { get; set; }

                public IEnumerable<EmployeeStatus> EmployeeStatuses { get; set; }
            }

            public class EmployeeStatus
            {
                public string ValidationStatus { get; set; }
                public int CompetencyCount { get; set; }
            }

            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid? DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid ProfileStandardIdentifier { get; set; }
            public string ProfileTitle { get; set; }
            public int CompetencyCount { get; set; }

            public IEnumerable<Employee> Employees { get; set; }
        }

        public class IndividualProfileAssignment
        {
            public string PersonName { get; set; }
            public string PrimaryProfileName { get; set; }
            public string SecondaryRequiredProfiles { get; set; }
            public string SecondaryProfiles { get; set; }
            public string Managers { get; set; }
            public string Supervisors { get; set; }
            public string Validators { get; set; }
        }

        public class MultiOrganizationUser
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string PersonCompanies { get; set; }
        }

        public class MultiOrganizationCompany
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string CompanyName { get; set; }
        }

        public class PhoneList
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
        }

        public class TrainingCompletionDate
        {
            public string CompanyName { get; set; }
            public string DepartmentName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string AchievementTitle { get; set; }
            public string AchievementLabel { get; set; }
            public DateTimeOffset? DateCompleted { get; set; }
            public DateTimeOffset? ExpirationDate { get; set; }
            public bool IsRequired { get; set; }
            public bool IsTimeSensitive { get; set; }
            public decimal? GradePercent { get; set; }
            public string CredentialStatus { get; set; }
        }

        public class TrainingExpiryDate
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public Guid AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public DateTimeOffset? DateCompleted { get; set; }
            public DateTimeOffset? ExpirationDate { get; set; }
            public bool IsRequired { get; set; }
            public bool IsTimeSensitive { get; set; }
            public string Status { get; set; }
            public bool IsQuizPassed { get; set; }
        }

        public class TrainingHistoryPerUser
        {
            public string PersonFullName { get; set; }
            public string ResourceTitle { get; set; }
            public DateTimeOffset? DateCompleted { get; set; }
            public DateTimeOffset? ExpirationDate { get; set; }
            public string AccreditorName { get; set; }
            public bool IsCompetent { get; set; }
            public decimal? Score { get; set; }
        }

        public class TrainingRequirementsPerUser
        {
            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }
            public Guid DepartmentIdentifier { get; set; }
            public string DepartmentName { get; set; }
            public Guid? UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string Number { get; set; }
            public string Summary { get; set; }
        }

        public class UserCompetencySummary
        {
            public class Profile
            {
                public Guid ProfileStandardIdentifier { get; set; }
                public string ProfileTitle { get; set; }
                public bool IsPrimary { get; set; }
                public string FullName { get; set; }
                public Guid DepartmentIdentifier { get; set; }

                public IEnumerable<Manager> Managers { get; set; }
                public IEnumerable<Status> Statuses { get; set; }
                public IEnumerable<StatusSummary> StatusesSummary { get; set; }

                public int SumEmployeeCompetencies { get; set; }
                public int SumTotalCompetencies { get; set; }
                public int AvgEmployeePercent { get; set; }
            }

            public class Manager
            {
                public string FullName { get; set; }
                public string EmailWork { get; set; }
                public string PhoneWork { get; set; }
            }

            public class Status
            {
                public int StatusSequence { get; set; }
                public string StatusName { get; set; }
                public int CompetencyCount { get; set; }
            }

            public class StatusSummary
            {
                public int EmployeeCompetencies { get; set; }
                public int TotalCompetencies { get; set; }
                public int EmployeePercent { get; set; }
            }

            public Guid OrganizationIdentifier { get; set; }
            public string CompanyName { get; set; }

            public IEnumerable<string> Areas { get; set; }
            public IEnumerable<Profile> Profiles { get; set; }
        }

        public class UserTrainingExpiryReminder
        {
            public string CompanyName { get; set; }
            public string AchievementTitle { get; set; }
            public string FullName { get; set; }
            public DateTimeOffset? ExpirationDate { get; set; }
        }

        #endregion

        #region Methods

        public static IEnumerable<UserTrainingExpiryReminder> SelectUserTrainingExpiryReminder(Guid[] departments, Guid[] achievements, bool? isRequired)
        {
            var days60 = DateTimeOffset.UtcNow.AddDays(-60);

            using (var db = new InternalDbContext())
            {
                var credentials = db.VCmdsCredentials
                    .Where(x =>
                        x.UserUtcArchived == null
                        && x.CredentialExpirationLifetimeQuantity != null
                        && x.CredentialExpirationLifetimeUnit == "Month"
                        && x.CredentialExpirationExpected > days60
                        && achievements.Contains(x.AchievementIdentifier)
                    );

                if (isRequired.HasValue)
                    credentials = credentials.Where(x => x.CredentialIsMandatory == isRequired);

                return credentials
                    .Join(
                        db.Memberships.Where(
                            x => x.Group.GroupType == GroupTypes.Department
                              && x.MembershipType != "Administration"
                              && departments.Contains(x.GroupIdentifier)),
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => new UserTrainingExpiryReminder
                        {
                            CompanyName = b.Group.Organization.CompanyTitle,
                            AchievementTitle = a.AchievementTitle,
                            FullName = a.UserFullName,
                            ExpirationDate = a.CredentialExpirationExpected
                        }
                    )
                    .OrderBy(x => x.FullName)
                    .ToList();
            }
        }

        public static IEnumerable<TrainingRequirementsPerUser> SelectTrainingRequirementsPerUser(Guid organization, string departments, string status)
        {
            var query = @"EXEC custom_cmds.SelectTrainingRequirementsPerUser @OrganizationIdentifier, @Departments, @Status";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organization),
                new SqlParameter("@Departments", departments),
                new SqlParameter("@Status", status)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 5 * 60;

                return db.Database.SqlQuery<TrainingRequirementsPerUser>(query, sqlParameters).ToList();
            }
        }

        public static IEnumerable<TrainingHistoryPerUser> SelectTrainingHistoryPerUser(
                Guid[] departments,
                Guid[] achievements,
                bool? isRequired
            )
        {
            using (var db = new InternalDbContext())
            {
                var credentials = db.VCmdsCredentials.Where(
                    x => x.UserUtcArchived == null
                      && db.Memberships.Any(
                          y => y.UserIdentifier == x.UserIdentifier
                            && departments.Contains(y.GroupIdentifier)
                            && y.MembershipType == "Department")
                      && achievements.Contains(x.AchievementIdentifier)
                );

                if (isRequired.HasValue)
                    credentials = credentials.Where(x => x.CredentialIsMandatory == isRequired);

                return credentials
                    .Select(x => new TrainingHistoryPerUser
                    {
                        PersonFullName = x.UserFullName,
                        ResourceTitle = x.AchievementTitle,
                        DateCompleted = x.CredentialGranted,
                        ExpirationDate = x.CredentialExpirationExpected,
                        AccreditorName = x.AuthorityName,
                        IsCompetent = x.CredentialGranted != null // && x.AchievementLabel != "Module"
                    })
                    .OrderBy(x => x.PersonFullName)
                    .ThenBy(x => x.ResourceTitle)
                    .ToList();
            }
        }

        public static IEnumerable<TrainingExpiryDate> SelectTrainingExpiryDates(Guid[] departments, Guid[] resources, bool? isRequired, decimal minimumPassingGrade)
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Departments", departments != null ? string.Join(",", departments) : (object)DBNull.Value),
                new SqlParameter("@Achievements", resources != null ? string.Join(",", resources) : (object)DBNull.Value),
                new SqlParameter("@IsRequired", isRequired ?? (object)DBNull.Value),
                new SqlParameter("@MinimumPassingGrade", minimumPassingGrade)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<TrainingExpiryDate>("EXEC custom_cmds.SelectTrainingExpiryDates @Departments, @Achievements, @IsRequired, @MinimumPassingGrade", sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<TrainingCompletionDate> SelectTrainingCompletionDates(
            Guid[] departments,
            Guid[] achievements,
            bool? isRequired,
            DateTimeRange credentialGranted,
            string credentialStatus,
            string membershipFunction,
            bool? includeSelfDeclaredCredentials)
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Departments", string.Join(",", departments)),
                new SqlParameter("@Achievements", string.Join(",", achievements)),
                new SqlParameter("@IsRequired", isRequired ?? (object)DBNull.Value),
                new SqlParameter("@CredentialGrantedStartDate", credentialGranted?.Since ?? (object)DBNull.Value),
                new SqlParameter("@CredentialGrantedEndDate", credentialGranted?.Before ?? (object)DBNull.Value),
                new SqlParameter("@CredentialStatus", string.IsNullOrEmpty(credentialStatus) ? (object)DBNull.Value : credentialStatus),
                new SqlParameter("@MembershipType", string.IsNullOrEmpty(membershipFunction) ? (object)DBNull.Value : membershipFunction),
                new SqlParameter("@IncludeSelfDeclaredCredentials", includeSelfDeclaredCredentials ?? (object)DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5; // 5 minutes

                return db.Database
                    .SqlQuery<TrainingCompletionDate>(
                        "EXEC custom_cmds.SelectTrainingCompletionDates" +
                        "  @Departments" +
                        ", @Achievements" +
                        ", @IsRequired" +
                        ", @CredentialGrantedStartDate" +
                        ", @CredentialGrantedEndDate" +
                        ", @CredentialStatus" +
                        ", @MembershipType" +
                        ", @IncludeSelfDeclaredCredentials"
                        , sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<PhoneList> SelectPhoneList(
            string departments,
            string subTypes,
            string roles,
            bool isApproved,
            Guid organizationIdentifier
            )
        {
            const string query = @"EXEC custom_cmds.SelectPhoneList @Departments, @RoleTypes, @Roles, @IsApproved, @OrganizationIdentifier";

            object[] sqlParameters =
            {
                new SqlParameter("@Departments", departments),
                new SqlParameter("@RoleTypes", subTypes),
                new SqlParameter("@Roles", roles),
                new SqlParameter("@IsApproved", isApproved),
                new SqlParameter("@OrganizationIdentifier", organizationIdentifier)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<PhoneList>(query, sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<MultiOrganizationUser> SelectMultiOrganizationUser(IEnumerable<Guid> companies)
        {
            var query = @"
SELECT
    corePerson.UserIdentifier
   ,corePerson.FullName
   ,custom_cmds.GetUserCompanies(corePerson.UserIdentifier) AS PersonCompanies
FROM
    identities.[User] AS corePerson
WHERE
    corePerson.UtcArchived IS NULL
    AND 1 < (
        SELECT COUNT(DISTINCT coreDepartment.OrganizationIdentifier)
        FROM
            contacts.Membership as coreRole
            INNER JOIN identities.Department AS coreDepartment ON coreDepartment.DepartmentIdentifier = coreRole.GroupIdentifier
        WHERE
            coreRole.UserIdentifier = corePerson.UserIdentifier
            AND coreRole.MembershipType IN ('Department', 'Company')
    )";

            if (companies != null)
            {
                var initQuery = new StringBuilder();

                initQuery.AppendLine("DECLARE @Companies TABLE (ID UNIQUEIDENTIFIER);");

                foreach (var organizationId in companies)
                    initQuery.Append("INSERT INTO @Companies VALUES ('").Append(organizationId).AppendLine("');");

                query = initQuery.ToString() + query + @"
    AND (
        corePerson.UserIdentifier IN (
            SELECT
                m.UserIdentifier
            FROM
                contacts.Membership AS m
                INNER JOIN identities.Department AS dep ON dep.DepartmentIdentifier = m.GroupIdentifier
            WHERE
                dep.OrganizationIdentifier IN (SELECT ID FROM @Companies)
                AND m.MembershipType IN ('Department', 'Company')
        )
    )";
            }

            query += @"
ORDER BY
    FullName";

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<MultiOrganizationUser>(query)
                    .ToList();
        }

        public static IEnumerable<MultiOrganizationCompany> SelectMultiOrganizationCompany(IEnumerable<Guid> companies)
        {
            var query = @"
SELECT
    p.UserIdentifier
   ,p.FullName
   ,Organization.CompanyTitle AS CompanyName
FROM
    accounts.QOrganization as Organization
    INNER JOIN identities.[User] AS p ON p.UserIdentifier IN (
        SELECT
            m.UserIdentifier
        FROM
            contacts.Membership AS m
            INNER JOIN identities.Department AS d ON d.DepartmentIdentifier = m.GroupIdentifier
        WHERE
            d.OrganizationIdentifier = Organization.OrganizationIdentifier
            AND m.MembershipType IN ('Department', 'Company')
    )
WHERE
    p.UtcArchived IS NULL
    AND 1 < (
        SELECT COUNT(DISTINCT g.OrganizationIdentifier)
        FROM
            contacts.Membership AS m
            INNER JOIN identities.Department AS g ON g.DepartmentIdentifier = m.GroupIdentifier
        WHERE
            m.UserIdentifier = p.UserIdentifier
            AND m.MembershipType IN ('Department', 'Company')
    )";

            if (companies != null)
            {
                var initQuery = new StringBuilder();

                initQuery.AppendLine("DECLARE @Companies TABLE (ID UNIQUEIDENTIFIER);");

                foreach (var organizationId in companies)
                    initQuery.Append("INSERT INTO @Companies VALUES ('").Append(organizationId).AppendLine("');");

                query = initQuery.ToString() + query + @"
    AND Organization.OrganizationIdentifier IN (SELECT ID FROM @Companies)";
            }

            query += @"
ORDER BY
    Organization.CompanyTitle
   ,p.FullName";

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<MultiOrganizationCompany>(query)
                    .ToList();
        }

        public static IEnumerable<IndividualProfileAssignment> SelectIndividualProfileAssignment(Guid department, string roleType)
        {
            const string query = @"
SELECT
    Organization.OrganizationIdentifier
   ,Organization.CompanyTitle AS CompanyName
   ,Departments.DepartmentIdentifier
   ,Departments.DepartmentName
   ,Users.UserIdentifier
   ,P.FullName AS PersonName
   ,PrimaryProfiles.ProfileTitle AS PrimaryProfileName
   ,RequiredProfiles.Profiles AS SecondaryRequiredProfiles
   ,Profiles.Profiles AS SecondaryProfiles
   ,Relationships.Managers
   ,Relationships.Supervisors
   ,Relationships.Validators
FROM
    custom_cmds.ActiveUser AS Users
    INNER JOIN contacts.Membership AS Memberships ON Memberships.UserIdentifier = Users.UserIdentifier
    INNER JOIN identities.Department AS Departments ON Departments.DepartmentIdentifier = Memberships.GroupIdentifier
    INNER JOIN accounts.QOrganization AS Organization ON Organization.OrganizationIdentifier = Departments.OrganizationIdentifier
    INNER JOIN contacts.QPerson as P on P.UserIdentifier = Users.UserIdentifier and P.OrganizationIdentifier = Organization.OrganizationIdentifier
    LEFT JOIN (
        custom_cmds.Employment AS Employments
        INNER JOIN custom_cmds.[Profile] AS PrimaryProfiles ON PrimaryProfiles.ProfileStandardIdentifier = Employments.ProfileStandardIdentifier
    ) ON Employments.OrganizationIdentifier = Organization.OrganizationIdentifier
         AND Employments.UserIdentifier = Users.UserIdentifier

    OUTER APPLY (
        SELECT STRING_AGG('• ' + ProfileTitle, CHAR(13) + CHAR(10)) WITHIN GROUP (ORDER BY ProfileTitle) AS Profiles
        FROM (
            SELECT
                Profiles.ProfileTitle
            FROM
                custom_cmds.UserProfile AS EmployeeProfiles
                INNER JOIN custom_cmds.[Profile] AS Profiles ON Profiles.ProfileStandardIdentifier = EmployeeProfiles.ProfileStandardIdentifier
            WHERE
                EmployeeProfiles.UserIdentifier = Users.UserIdentifier
                AND EmployeeProfiles.DepartmentIdentifier = Departments.DepartmentIdentifier
                AND (
                    Employments.ProfileStandardIdentifier IS NULL
                    OR Employments.ProfileStandardIdentifier != EmployeeProfiles.ProfileStandardIdentifier
                )
                AND EmployeeProfiles.IsComplianceRequired = 1
        ) AS t
    ) AS RequiredProfiles

    OUTER APPLY (
        SELECT STRING_AGG('• ' + ProfileTitle, CHAR(13) + CHAR(10)) WITHIN GROUP (ORDER BY ProfileTitle) AS Profiles
        FROM (
            SELECT
                Profiles.ProfileTitle
            FROM
                custom_cmds.UserProfile AS EmployeeProfiles
                INNER JOIN custom_cmds.[Profile] AS Profiles ON Profiles.ProfileStandardIdentifier = EmployeeProfiles.ProfileStandardIdentifier
            WHERE
                EmployeeProfiles.UserIdentifier = Users.UserIdentifier
                AND EmployeeProfiles.DepartmentIdentifier = Departments.DepartmentIdentifier
                AND (
                    Employments.ProfileStandardIdentifier IS NULL
                    OR Employments.ProfileStandardIdentifier != EmployeeProfiles.ProfileStandardIdentifier
                )
                AND EmployeeProfiles.IsComplianceRequired = 0
        ) AS t
    ) AS Profiles

    OUTER APPLY  (
        SELECT
            STRING_AGG(
                CASE
                    WHEN UserConnection.IsManager = 1 THEN '• ' + Persons.FullName
                    ELSE NULL
                END
               ,CHAR(13) + CHAR(10)
            ) WITHIN GROUP (ORDER BY Persons.FullName) AS Managers
           ,STRING_AGG(
                CASE
                    WHEN UserConnection.IsSupervisor = 1 THEN '• ' + Persons.FullName
                    ELSE NULL
                END
               ,CHAR(13) + CHAR(10)
            ) WITHIN GROUP (ORDER BY Persons.FullName) AS Supervisors
           ,STRING_AGG(
                CASE
                    WHEN UserConnection.IsValidator = 1 THEN '• ' + Persons.FullName
                    ELSE NULL
                END
               ,CHAR(13) + CHAR(10)
            ) WITHIN GROUP (ORDER BY Persons.FullName) AS Validators
        FROM
            identities.UserConnection
            INNER JOIN identities.[User] AS Persons ON Persons.UserIdentifier = UserConnection.FromUserIdentifier
        WHERE
        UserConnection.ToUserIdentifier = Users.UserIdentifier
        AND Persons.UtcArchived IS NULL
    ) AS Relationships

WHERE
    Memberships.GroupIdentifier = @DepartmentIdentifier
    AND Memberships.MembershipType IN (SELECT ItemText FROM dbo.SplitText(@RoleType, ','))";

            var sqlParameters = new[]
            {
                new SqlParameter("@DepartmentIdentifier", department),
                new SqlParameter("@RoleType", roleType)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<IndividualProfileAssignment>(query, sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<CompetencyStatusHistoryChart> SelectZUserStatusHistory(
            Guid? organization,
            Guid[] departments,
            Guid? user,
            DateTime startDate,
            DateTime endDate,
            string option
        )
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrganizationIdentifier", organization ?? (object)DBNull.Value),
                new SqlParameter("@Departments", departments.IsEmpty() ? DBNull.Value : (object)string.Join(",", departments)),
                new SqlParameter("@UserIdentifier", user ?? (object)DBNull.Value),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@Option", option),
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database
                    .SqlQuery<CompetencyStatusHistoryChart>("EXEC custom_cmds.SelectZUserStatusHistory @OrganizationIdentifier, @Departments, @UserIdentifier, @StartDate, @EndDate, @Option;", sqlParameters)
                    .ToList();
            }
        }

        public static CompetencyPerProfile[] SelectCompetencyPerProfile(Guid profileStandardIdentifier)
        {
            #region Query

            const string query = @"
SELECT p.ProfileNumber,
       p.ProfileTitle AS ProfileTitle,
       c.Number AS CompetencyNumber,
       c.Summary AS CompetencySummary,
       c.Knowledge AS CompetencyKnowledge,
       c.Skills AS CompetencySkills,
       c.ProgramHours
FROM custom_cmds.ProfileCompetency AS pc
    INNER JOIN custom_cmds.Competency AS c
        ON pc.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.[Profile] AS p
        ON pc.ProfileStandardIdentifier = p.ProfileStandardIdentifier
WHERE p.ProfileStandardIdentifier = @ProfileStandardIdentifier
      AND c.IsDeleted = 0
ORDER BY c.Number;";

            #endregion

            var sqlParameters = new object[]
            {
                new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier),
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<CompetencyPerProfile>(query, sqlParameters)
                    .ToArray();
            }
        }

        public static IEnumerable<CollegeCertificate> SelectReport024(
            Guid[] departments,
            Guid[] employees,
            string authorityName,
            string certificateType
        )
        {
            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@Departments",  CsvConverter.ConvertListToCsvText(departments)),
                new SqlParameter("@Employees", CsvConverter.ConvertListToCsvText(employees)),
                new SqlParameter("@AuthorityName", authorityName ?? (object)DBNull.Value),
                new SqlParameter("@CertificateType", certificateType ?? (object)DBNull.Value),
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database
                    .SqlQuery<CollegeCertificate>("EXEC custom_cmds.SelectCollegeCertificateEligibility @Departments, @Employees, @CertificateType, @AuthorityName;", sqlParameters)
                    .ToList();
            }

        }

        public static IEnumerable<EmployeeComplianceHistory> SelectEmployeeComplianceHistory(
            Guid organizationId,
            Guid[] departments,
            Guid[] employees,
            int[] sequences,
            DateTime startDate,
            DateTime endDate,
            int option,
            bool excludeUsersWithoutProfile
        )
        {
            const string query = "EXEC custom_cmds.SelectZUserStatus @OrganizationIdentifier, @Departments, @Employees, @Sequences, @StartDate, @EndDate, @Option, @ExcludeUsersWithoutProfile";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                SqlParameterHelper.PrimaryKey("@Departments", departments),
                SqlParameterHelper.PrimaryKey("@Employees", employees),
                SqlParameterHelper.PrimaryKey("@Sequences", sequences),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@Option", option),
                new SqlParameter("@ExcludeUsersWithoutProfile", excludeUsersWithoutProfile)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database
                    .SqlQuery<EmployeeComplianceHistory>(query, sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<EmployeeComplianceHistoryChart> SelectEmployeeComplianceHistoryChart(
            Guid organizationId,
            Guid[] departments,
            Guid[] employees,
            int[] sequences,
            DateTime startDate,
            DateTime endDate,
            int option,
            bool excludeUsersWithoutProfile
        )
        {
            const string query = "EXEC custom_cmds.SelectZUserStatusChart @OrganizationIdentifier, @Departments, @Employees, @Sequences, @StartDate, @EndDate, @Option, @ExcludeUsersWithoutProfile";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                SqlParameterHelper.PrimaryKey("@Departments", departments),
                SqlParameterHelper.PrimaryKey("@Employees", employees),
                SqlParameterHelper.PrimaryKey("@Sequences", sequences),
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@Option", option),
                new SqlParameter("@ExcludeUsersWithoutProfile", excludeUsersWithoutProfile)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database
                    .SqlQuery<EmployeeComplianceHistoryChart>(query, sqlParameters)
                    .ToList();
            }
        }

        public static List<EmployeeComplianceDepartment> SelectEmployeeComplianceDepartment(
            Guid organizationId,
            Guid[] departments,
            Guid[] employees,
            int[] sequences,
            DateTime? startDate,
            DateTime? endDate,
            int option,
            bool excludeUsersWithoutProfile,
            bool accessDepartment, bool accessCompany, bool accessAdministration
        )
        {
            const string query = "EXEC custom_cmds.SelectQUserStatusPerDepartment @OrganizationIdentifier, @Departments, @Employees, @Sequences, @StartDate, @EndDate, @Option, @ExcludeUsersWithoutProfile, @AccessDepartment, @AccessCompany, @AccessAdministration";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                SqlParameterHelper.PrimaryKey("@Departments", departments),
                SqlParameterHelper.PrimaryKey("@Employees", employees),
                SqlParameterHelper.PrimaryKey("@Sequences", sequences),
                new SqlParameter("@StartDate", startDate.HasValue ? startDate : (object)DBNull.Value),
                new SqlParameter("@EndDate", endDate.HasValue ? endDate : (object)DBNull.Value),
                new SqlParameter("@Option", option),
                new SqlParameter("@ExcludeUsersWithoutProfile", excludeUsersWithoutProfile),
                new SqlParameter("@AccessDepartment", accessDepartment),
                new SqlParameter("@AccessCompany", accessCompany),
                new SqlParameter("@AccessAdministration", accessAdministration)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database
                    .SqlQuery<EmployeeComplianceDepartment>(query, sqlParameters)
                    .ToList();
            }
        }

        public static IEnumerable<TrainingRequirementPerCompetency> SelectTrainingRequirementsPerCompetency(Guid[] departments, string status)
        {
            const string query = "EXEC custom_cmds.SelectTrainingRequirementsPerCompetency @Departments, @Status";

            var sqlParameters = new SqlParameter[]
            {
                SqlParameterHelper.PrimaryKey("@Departments", departments),
                new SqlParameter("@Status", status)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database.SqlQuery<TrainingRequirementPerCompetency>(query, sqlParameters).ToList();
            }
        }

        public static bool HasComplianceSummary()
        {
            const string query = "SELECT TOP 1 1 FROM custom_cmds.QUserStatus";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<int>(query).Count() > 0;
            }
        }

        public static IEnumerable<ComplianceSummary> SelectComplianceSummary(
            Guid organizationId,
            Guid[] departments,
            Guid[] employees,
            int[] sequences,
            int option,
            bool excludeUsersWithoutProfile
        )
        {
            const string query = @"
EXEC custom_cmds.SelectQUserStatus
    @OrganizationIdentifier
   ,@Departments
   ,@Users
   ,@Sequences
   ,@Option
   ,@ExcludeUsersWithoutProfile
";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                SqlParameterHelper.PrimaryKey("@Departments", departments),
                SqlParameterHelper.PrimaryKey("@Users", employees),
                SqlParameterHelper.PrimaryKey("@Sequences", sequences),
                new SqlParameter("@Option", option),
                new SqlParameter("@ExcludeUsersWithoutProfile", excludeUsersWithoutProfile),
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;

                return db.Database.SqlQuery<ComplianceSummary>(query, sqlParameters).ToList();
            }
        }

        public static IEnumerable<DepartmentCompetency> SelectDepartmentCompetencies(
            Guid department,
            Guid? profileStandardIdentifier,
            bool? isTimeSensitive,
            string criticality,
            bool priorityMustBeNull,
            bool showExcludedCompetencies
        )
        {
            const string query = @"
EXEC custom_cmds.SelectDepartmentCompetencies
  @DepartmentIdentifier = @DepartmentIdentifier
 ,@ProfileStandardIdentifier = @ProfileStandardIdentifier
 ,@IsTimeSensitive = @IsTimeSensitive
 ,@Criticality = @Criticality
 ,@PriorityMustBeNull = @PriorityMustBeNull
 ,@ShowExcludedCompetencies = @ShowExcludedCompetencies";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentIdentifier", (object)department),
                new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier.HasValue ? (object)profileStandardIdentifier : DBNull.Value),
                new SqlParameter("@IsTimeSensitive", isTimeSensitive.HasValue ? (object)isTimeSensitive : DBNull.Value),
                new SqlParameter("@Criticality", !string.IsNullOrEmpty(criticality) ? (object)criticality : DBNull.Value),
                new SqlParameter("@PriorityMustBeNull", priorityMustBeNull),
                new SqlParameter("@ShowExcludedCompetencies", showExcludedCompetencies)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60 * 5;// in seconds

                return db.Database.SqlQuery<DepartmentCompetency>(query, parameters).ToList();
            }
        }

        public static bool HasBillableUsersForInSite(DateTime fromDate, DateTime thruDate)
        {
            using (var db = new InternalDbContext())
            {
                return db.QInvoiceFees.FirstOrDefault(x => x.FromDate == fromDate && x.ThruDate == thruDate) != null;
            }
        }

        public static void PrepareInvoice(DateTime fromDate, DateTime thruDate, decimal unitPricePerPeriodClassA)
        {
            const string query = @"EXEC custom_cmds.RefreshQInvoice @FromDate, @ThruDate, @UnitPricePerPeriodClassA";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ThruDate", thruDate),
                new SqlParameter("@UnitPricePerPeriodClassA", unitPricePerPeriodClassA)
            };

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 10 * 60;
                db.Database.ExecuteSqlCommand(query, parameters);
            }
        }

        public static List<BillableUser> SelectBillableUsersForInSite(
            DateTime fromDate,
            DateTime thruDate,
            decimal unitPricePerPeriodClassA,
            decimal unitPricePerPeriodClassB,
            decimal unitPricePerPeriodClassC
        )
        {
            const string query = @"
EXEC custom_cmds.SelectBillableUsers
     @FromDate
    ,@ThruDate
    ,@UnitPricePerPeriodClassA
    ,@UnitPricePerPeriodClassB
    ,@UnitPricePerPeriodClassC;";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ThruDate", thruDate),
                new SqlParameter("@UnitPricePerPeriodClassA", unitPricePerPeriodClassA),
                new SqlParameter("@UnitPricePerPeriodClassB", unitPricePerPeriodClassB),
                new SqlParameter("@UnitPricePerPeriodClassC", unitPricePerPeriodClassC)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<BillableUser>(query, parameters).ToList();
            }
        }

        #endregion

        #region CompanySummary

        public static IEnumerable<CompanySummary> SelectCompanySummary(Guid? organizationId)
        {
            #region Query

            var query = @"
SELECT
    c.OrganizationIdentifier
   ,c.CompanyTitle AS [Name]
   ,COUNT(DISTINCT p.UserIdentifier) AS UserCount
FROM
    accounts.QOrganization AS c
    INNER JOIN identities.Department AS d ON c.OrganizationIdentifier = d.OrganizationIdentifier
    INNER JOIN contacts.Membership AS m ON d.DepartmentIdentifier = m.GroupIdentifier
    INNER JOIN identities.[User] AS p ON m.UserIdentifier = p.UserIdentifier
WHERE
    (@OrganizationIdentifier IS NULL OR c.OrganizationIdentifier = @OrganizationIdentifier)
    AND c.AccountClosed IS NULL
    AND p.UtcArchived IS NULL
GROUP BY
    c.OrganizationIdentifier
   ,c.CompanyTitle
ORDER BY
    c.CompanyTitle";

            #endregion

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId ?? (object) DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                var list = db.Database
                    .SqlQuery<CompanySummary>(query, sqlParameters)
                    .ToList();

                foreach (var item in list)
                {
                    item.Departments = SelectCompanySummaryDepartments(item.OrganizationIdentifier, db);
                    // item.Users = SelectCompanySummaryUsers(item.OrganizationIdentifier, db);
                    item.Roles = SelectCompanySummaryRoles(item.OrganizationIdentifier, db);
                }

                return list;
            }
        }

        private static IEnumerable<CompanySummary.Department> SelectCompanySummaryDepartments(Guid organizationId, InternalDbContext db)
        {
            const string query = @"
SELECT department.DepartmentIdentifier
      ,department.DepartmentName
      ,COUNT(*) AS UserCount
  FROM identities.Department
       INNER JOIN contacts.Membership AS m
         ON m.GroupIdentifier = department.DepartmentIdentifier
       INNER JOIN identities.[User] AS p
         ON p.UserIdentifier = m.UserIdentifier
  WHERE department.OrganizationIdentifier = @OrganizationIdentifier
    AND p.UtcArchived IS NULL
  GROUP BY department.DepartmentIdentifier, department.DepartmentName
  ORDER BY department.DepartmentName;";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId)
            };

            return db.Database
                .SqlQuery<CompanySummary.Department>(query, sqlParameters)
                .ToList();
        }

        private static IEnumerable<CompanySummary.Role> SelectCompanySummaryRoles(Guid organizationId, InternalDbContext db)
        {
            const string query = @"
SELECT
    CASE
        WHEN r.GroupIdentifier IS NOT NULL THEN 0
        ELSE 1
    END AS [Sequence]
   ,ISNULL(r.GroupName,'Without Any Role') AS RoleName
   ,COUNT(*) AS UserCount
FROM
    identities.[User] AS c
    LEFT JOIN custom_cmds.UserRole AS r ON r.UserIdentifier = c.UserIdentifier
WHERE
    c.UtcArchived IS NULL
    AND c.UserIdentifier IN (
        SELECT
            m.UserIdentifier
        FROM
            contacts.Membership AS m
            INNER JOIN identities.Department AS g
                ON g.DepartmentIdentifier = m.GroupIdentifier
        WHERE
            g.OrganizationIdentifier = @OrganizationIdentifier
    )
GROUP BY
    r.GroupIdentifier,r.GroupName
ORDER BY
    [Sequence]
   ,RoleName;";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId)
            };

            return db.Database
                .SqlQuery<CompanySummary.Role>(query, sqlParameters)
                .ToList();
        }

        #endregion

        #region DepartmentProfileSummary

        public static IEnumerable<DepartmentProfileSummary> SelectDepartmentProfileSummary(Guid? organizationId, Guid? department, Guid? profileStandardIdentifier)
        {
            var query = @"
SELECT Organization.OrganizationIdentifier,
       Organization.CompanyTitle AS CompanyName,
       dep.DepartmentIdentifier,
       dep.DepartmentName,
       p.ProfileStandardIdentifier,
       p.ProfileTitle AS ProfileTitle
FROM accounts.QOrganization AS Organization
    INNER JOIN identities.Department AS dep
        ON dep.OrganizationIdentifier = Organization.OrganizationIdentifier
    INNER JOIN custom_cmds.DepartmentProfile AS dp
        ON dp.DepartmentIdentifier = dep.DepartmentIdentifier
    INNER JOIN custom_cmds.[Profile] AS p
        ON p.ProfileStandardIdentifier = dp.ProfileStandardIdentifier
WHERE
      (
          @OrganizationIdentifier IS NULL
          OR Organization.OrganizationIdentifier = @OrganizationIdentifier
      )
      AND
      (
          @DepartmentIdentifier IS NULL
          OR dep.DepartmentIdentifier = @DepartmentIdentifier
      )
      AND
      (
          @ProfileStandardIdentifier IS NULL
          OR p.ProfileStandardIdentifier = @ProfileStandardIdentifier
      )
UNION ALL
SELECT Organization.OrganizationIdentifier,
       Organization.CompanyTitle AS CompanyName,
       NULL AS DepartmentIdentifier,
       NULL AS DepartmentName,
       p.ProfileStandardIdentifier,
       p.ProfileTitle AS ProfileTitle
FROM accounts.QOrganization AS Organization
    INNER JOIN custom_cmds.VCmdsProfileOrganization cp
        ON cp.OrganizationIdentifier = Organization.OrganizationIdentifier
    INNER JOIN custom_cmds.[Profile] p
        ON p.ProfileStandardIdentifier = cp.ProfileStandardIdentifier
WHERE (
          @OrganizationIdentifier IS NULL
          OR Organization.OrganizationIdentifier = @OrganizationIdentifier
      )
      AND
      (
          @ProfileStandardIdentifier IS NULL
          OR p.ProfileStandardIdentifier = @ProfileStandardIdentifier
      )
      AND @DepartmentIdentifier IS NULL
      AND p.ProfileStandardIdentifier NOT IN
          (
              SELECT ProfileStandardIdentifier
              FROM custom_cmds.DepartmentProfile AS dp
                  INNER JOIN identities.Department AS dep
                      ON dep.DepartmentIdentifier = dp.DepartmentIdentifier
              WHERE dep.OrganizationIdentifier = Organization.OrganizationIdentifier
          )
ORDER BY CompanyName,
         DepartmentName,
         ProfileTitle";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId ?? (object) DBNull.Value),
                new SqlParameter("@DepartmentIdentifier", department ?? (object) DBNull.Value),
                new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier ?? (object) DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                var list = db.Database
                    .SqlQuery<DepartmentProfileSummary>(query, sqlParameters)
                    .ToList();

                foreach (var item in list)
                {
                    item.CompetencyCount = item.DepartmentIdentifier.HasValue ? CountDepartmentProfileSummaryCompetency(item.DepartmentIdentifier.Value, item.ProfileStandardIdentifier) : 0;
                    item.Employees = item.DepartmentIdentifier.HasValue ? SelectDepartmentProfileSummaryEmployees(item.DepartmentIdentifier.Value, item.ProfileStandardIdentifier) : new List<DepartmentProfileSummary.Employee>();
                }

                return list;
            }
        }

        private static IEnumerable<DepartmentProfileSummary.Employee> SelectDepartmentProfileSummaryEmployees(Guid department, Guid profile)
        {
            const string query = @"
SELECT DISTINCT ep.ProfileStandardIdentifier AS ProfileStandardIdentifier
              , u.UserIdentifier
              , P.FullName AS PersonFullName
FROM custom_cmds.UserProfile AS ep
         INNER JOIN contacts.Membership AS m ON m.UserIdentifier = ep.UserIdentifier
         INNER JOIN contacts.QGroup AS G ON G.GroupIdentifier = m.GroupIdentifier
         INNER JOIN accounts.QOrganization AS O ON O.OrganizationIdentifier = G.OrganizationIdentifier
         INNER JOIN custom_cmds.ActiveUser AS u ON u.UserIdentifier = m.UserIdentifier
         INNER JOIN contacts.QPerson AS P
                    ON P.UserIdentifier = u.UserIdentifier AND P.OrganizationIdentifier = O.OrganizationIdentifier

WHERE
    ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
    AND m.GroupIdentifier = @DepartmentIdentifier

ORDER BY PersonFullName
;";

            var sqlParameters = new[]
            {
                new SqlParameter("@DepartmentIdentifier", department),
                new SqlParameter("@ProfileStandardIdentifier", profile)
            };

            using (var db = new InternalDbContext())
            {
                var array = db.Database
                    .SqlQuery<DepartmentProfileSummary.Employee>(query, sqlParameters)
                    .ToArray();

                foreach (var item in array)
                    item.EmployeeStatuses = SelectDepartmentProfileSummaryEmployeeStatus(item.ProfileStandardIdentifier, item.UserIdentifier, department);

                return array;
            }
        }

        private static int CountDepartmentProfileSummaryCompetency(Guid department, Guid profile)
        {
            var query = @"
SELECT COUNT(q.CompetencyStandardIdentifier) AS CompetencyCount
FROM identities.Department AS s
     OUTER APPLY
(
    SELECT DISTINCT
           ep.UserIdentifier
         , ec.CompetencyStandardIdentifier
    FROM custom_cmds.UserProfile                            AS ep
         INNER JOIN custom_cmds.ProfileCompetency           AS pc ON pc.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
         INNER JOIN custom_cmds.UserCompetency              AS ec ON ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                                                                     AND ec.UserIdentifier = ep.UserIdentifier
         INNER JOIN custom_cmds.DepartmentProfileCompetency AS cs ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                                                                     AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
         INNER JOIN contacts.Membership                     AS m ON m.UserIdentifier = ec.UserIdentifier
                                                                     AND m.GroupIdentifier = cs.DepartmentIdentifier
         INNER JOIN identities.[User]                       AS CmdsContact ON CmdsContact.UserIdentifier = m.UserIdentifier
         INNER JOIN contacts.Person                         AS P ON P.UserIdentifier = CmdsContact.UserIdentifier
                                                                    AND P.UserAccessGranted IS NOT NULL
    WHERE cs.DepartmentIdentifier = @DepartmentIdentifier
          AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
          AND CmdsContact.UtcArchived IS NULL
)                          AS q
WHERE s.DepartmentIdentifier = @DepartmentIdentifier";

            var sqlParameters = new[]
            {
                new SqlParameter("@DepartmentIdentifier", department),
                new SqlParameter("@ProfileStandardIdentifier", profile)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<int>(query, sqlParameters)
                    .Single();
            }
        }

        private static IEnumerable<DepartmentProfileSummary.EmployeeStatus> SelectDepartmentProfileSummaryEmployeeStatus(Guid profile, Guid user, Guid department)
        {
            var query = @"
SELECT ec.ValidationStatus
     , COUNT(DISTINCT ec.CompetencyStandardIdentifier) AS CompetencyCount
FROM custom_cmds.UserProfile                            AS ep
     INNER JOIN custom_cmds.ProfileCompetency           AS pc ON pc.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
     INNER JOIN custom_cmds.UserCompetency              AS ec ON ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                                                                 AND ec.UserIdentifier = ep.UserIdentifier
     INNER JOIN custom_cmds.DepartmentProfileCompetency cs ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                                                              AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
WHERE ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
      AND ep.UserIdentifier = @UserIdentifier
      AND cs.DepartmentIdentifier = @DepartmentIdentifier
GROUP BY ec.ValidationStatus
ORDER BY ec.ValidationStatus";

            var sqlParameters = new[]
            {
                new SqlParameter("@ProfileStandardIdentifier", profile),
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@DepartmentIdentifier", department)
            };

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<DepartmentProfileSummary.EmployeeStatus>(query, sqlParameters)
                    .ToList();
            }
        }

        #endregion

        #region UserCompetencySummary

        public static IEnumerable<UserCompetencySummary> SelectUserCompetencySummary(Guid userKey, Guid? profileStandardIdentifier, bool isPrimaryOnly)
        {
            var query = @"
SELECT Organization.OrganizationIdentifier,
       Organization.CompanyTitle AS CompanyName
FROM accounts.QOrganization AS Organization
WHERE 
      (
          Organization.OrganizationIdentifier IN
             (
                 SELECT dep.OrganizationIdentifier
                 FROM identities.Department AS dep
                     INNER JOIN contacts.Membership AS m
                         ON m.GroupIdentifier = dep.DepartmentIdentifier
                 WHERE m.UserIdentifier = @UserIdentifier
             )
      )
      AND Organization.OrganizationIdentifier IN
          (
              SELECT department.OrganizationIdentifier
              FROM custom_cmds.UserProfile AS ep
                  INNER JOIN identities.Department
                      ON department.DepartmentIdentifier = ep.DepartmentIdentifier
              WHERE ep.UserIdentifier = @UserIdentifier
                    AND
                    (
                        @ProfileStandardIdentifier IS NULL
                        OR ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
                    )
                    AND
                    (
                        @DepartmentIdentifier IS NULL
                        OR ep.DepartmentIdentifier = @DepartmentIdentifier
                    )
          )
ORDER BY Organization.CompanyTitle";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", userKey),
                new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier ?? (object)DBNull.Value),
                new SqlParameter("@DepartmentIdentifier", DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                var array = db.Database
                    .SqlQuery<UserCompetencySummary>(query, sqlParameters)
                    .ToArray();

                foreach (var item in array)
                {
                    item.Areas = SelectUserCompetencySummaryAreas(userKey, item.OrganizationIdentifier);
                    item.Profiles = SelectUserCompetencySummaryProfiles(userKey, item.OrganizationIdentifier, profileStandardIdentifier, null);

                    if (isPrimaryOnly)
                        item.Profiles = item.Profiles.Where(x => x.IsPrimary);
                }

                return array;
            }
        }

        private static IEnumerable<string> SelectUserCompetencySummaryAreas(Guid userKey, Guid organizationId)
        {
            var query = @"
SELECT
    Department.DepartmentName
FROM
    identities.Department
    INNER JOIN contacts.Membership AS m
        ON m.GroupIdentifier = department.DepartmentIdentifier
WHERE
    Department.OrganizationIdentifier = @OrganizationIdentifier
    AND m.UserIdentifier = @UserIdentifier
ORDER BY
    Department.DepartmentName";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", userKey),
                new SqlParameter("@OrganizationIdentifier", organizationId)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<string>(query, sqlParameters)
                    .ToArray();
        }

        private static IEnumerable<UserCompetencySummary.Profile> SelectUserCompetencySummaryProfiles(Guid userKey, Guid organizationId, Guid? profileStandardIdentifier, Guid? department)
        {
            var query = @"
SELECT p.ProfileStandardIdentifier
      ,p.ProfileTitle AS ProfileTitle
      ,CASE
         WHEN e.UserIdentifier IS NOT NULL
           THEN CAST(1 AS BIT)
         ELSE CAST(0 AS BIT)
       END AS IsPrimary
      ,COALESCE(Person.FullName,employee.FullName) AS FullName
      ,ep.DepartmentIdentifier
  FROM custom_cmds.UserProfile AS ep
       INNER JOIN identities.Department
         ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
       INNER JOIN custom_cmds.[Profile] AS p
         ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
       INNER JOIN custom_cmds.ActiveUser AS employee
         ON employee.UserIdentifier = ep.UserIdentifier
       INNER JOIN contacts.QPerson AS Person 
         ON Person.OrganizationIdentifier = Department.OrganizationIdentifier
            AND Person.UserIdentifier = employee.UserIdentifier
       LEFT JOIN custom_cmds.Employment AS e
         ON e.UserIdentifier = ep.UserIdentifier
            AND e.DepartmentIdentifier = ep.DepartmentIdentifier
            AND e.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
  WHERE ep.UserIdentifier = @UserIdentifier
    AND Department.OrganizationIdentifier = @OrganizationIdentifier
    AND (@ProfileStandardIdentifier IS NULL OR ep.ProfileStandardIdentifier = @ProfileStandardIdentifier)
    AND (@DepartmentIdentifier IS NULL OR ep.DepartmentIdentifier = @DepartmentIdentifier)
  ORDER BY IsPrimary DESC
          ,p.ProfileTitle";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", userKey),
                new SqlParameter("@OrganizationIdentifier", organizationId),
                new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier ?? (object)DBNull.Value),
                new SqlParameter("@DepartmentIdentifier", department ?? (object)DBNull.Value)
            };

            using (var db = new InternalDbContext())
            {
                var profiles = db.Database
                    .SqlQuery<UserCompetencySummary.Profile>(query, sqlParameters)
                    .ToArray();

                foreach (var profile in profiles)
                {
                    profile.Managers = SelectUserCompetencySummaryManagers(userKey, organizationId);
                    profile.Statuses = SelectUserCompetencySummaryStatuses(userKey, profile.ProfileStandardIdentifier, profile.DepartmentIdentifier);
                    profile.StatusesSummary = SelectUserCompetencySummaryStatusSummaries(userKey, profile.ProfileStandardIdentifier, profile.DepartmentIdentifier);

                    profile.SumEmployeeCompetencies = profile.StatusesSummary.Sum(x => x.EmployeeCompetencies);
                    profile.SumTotalCompetencies = profile.StatusesSummary.Sum(x => x.TotalCompetencies);
                    profile.AvgEmployeePercent = (int)Math.Round(profile.StatusesSummary.Average(x => x.EmployeePercent), 0);
                }

                return profiles;
            }
        }

        private static IEnumerable<UserCompetencySummary.Manager> SelectUserCompetencySummaryManagers(Guid userKey, Guid organizationId)
        {
            var query = @"
SELECT DISTINCT
    p.FullName
   ,p.Email AS EmailWork
   ,Person.PhoneWork
FROM
    custom_cmds.ActiveUser AS p
    INNER JOIN contacts.Person
        ON Person.UserIdentifier = p.UserIdentifier
            AND Person.OrganizationIdentifier = @OrganizationIdentifier
    INNER JOIN identities.UserConnection AS r
        ON r.FromUserIdentifier = p.UserIdentifier
    INNER JOIN contacts.Membership AS m
        ON m.UserIdentifier = r.FromUserIdentifier
    INNER JOIN identities.Department AS g
        ON g.DepartmentIdentifier = m.GroupIdentifier
WHERE
    r.ToUserIdentifier = @UserIdentifier
    AND g.OrganizationIdentifier = @OrganizationIdentifier
    AND r.IsValidator = 1
ORDER BY
    p.FullName;";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", userKey),
                new SqlParameter("@OrganizationIdentifier", organizationId)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<UserCompetencySummary.Manager>(query, sqlParameters)
                    .ToArray();
        }

        private static IEnumerable<UserCompetencySummary.Status> SelectUserCompetencySummaryStatuses(Guid user, Guid? profile, Guid? department)
        {
            var query = @"
SELECT
           v.ItemNumber                             AS StatusSequence
         , CASE WHEN v.ItemName = 'Not Applicable' AND ec.IsValidated = 1 THEN 'Validated Not Applicable' ELSE v.ItemName end                               AS StatusName
         , COUNT(DISTINCT ec.CompetencyStandardIdentifier) AS CompetencyCount
FROM
           identities.[User] AS s
CROSS APPLY
           (
               SELECT
                   TCollectionItem.*
               FROM
                   utilities.TCollectionItem
                   INNER JOIN utilities.TCollection ON TCollection.CollectionIdentifier = TCollectionItem.CollectionIdentifier
               WHERE
                   TCollection.CollectionName = 'Validations/Verification/Status'
           )                 AS v
LEFT JOIN  (custom_cmds.ProfileCompetency               AS pc
INNER      JOIN custom_cmds.UserProfile                 AS ep
                ON ep.ProfileStandardIdentifier = pc.ProfileStandardIdentifier

INNER      JOIN custom_cmds.UserCompetency              AS ec
                ON ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                   AND ec.UserIdentifier = ep.UserIdentifier

INNER      JOIN custom_cmds.DepartmentProfileCompetency AS cs
                ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                   AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
                   AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier

INNER      JOIN identities.Department
                ON Department.DepartmentIdentifier = cs.DepartmentIdentifier)
           ON pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
              AND ec.ValidationStatus = v.ItemName
              AND ep.UserIdentifier = @UserIdentifier
              AND ep.DepartmentIdentifier = @DepartmentIdentifier
WHERE
           s.UserIdentifier = @UserIdentifier
GROUP BY
           v.ItemNumber
         , CASE WHEN v.ItemName = 'Not Applicable' AND ec.IsValidated = 1 THEN 'Validated Not Applicable' ELSE v.ItemName end";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@ProfileStandardIdentifier", profile ?? (object)DBNull.Value),
                new SqlParameter("@DepartmentIdentifier", department ?? (object)DBNull.Value)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<UserCompetencySummary.Status>(query, sqlParameters)
                    .ToArray();
        }

        private static IEnumerable<UserCompetencySummary.StatusSummary> SelectUserCompetencySummaryStatusSummaries(Guid user, Guid? profile, Guid? department)
        {
            var query = @"
SELECT COUNT(DISTINCT ec.CompetencyStandardIdentifier) AS EmployeeCompetencies,
       COUNT(DISTINCT pc.CompetencyStandardIdentifier) AS TotalCompetencies,
       CASE
           WHEN COUNT(DISTINCT pc.CompetencyStandardIdentifier) = 0 THEN
               0
           ELSE
               ROUND(100 * COUNT(DISTINCT ec.CompetencyStandardIdentifier) / COUNT(DISTINCT pc.CompetencyStandardIdentifier), 0)
       END AS EmployeePercent
FROM identities.Department AS s
    LEFT JOIN(custom_cmds.ProfileCompetency AS pc
    INNER JOIN custom_cmds.UserProfile AS ep
        ON ep.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
           AND ep.UserIdentifier = @UserIdentifier
    INNER JOIN custom_cmds.DepartmentProfileCompetency AS cs
        ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
           AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
           AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
    LEFT JOIN custom_cmds.UserCompetency AS ec
        ON ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
           AND ec.UserIdentifier = @UserIdentifier
           AND (ec.IsValidated = 1 AND ec.ValidationStatus IN ('Validated','Not Applicable')))
        ON pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
           AND ep.UserIdentifier = @UserIdentifier
           AND ep.DepartmentIdentifier = @DepartmentIdentifier
           AND cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
WHERE s.DepartmentIdentifier = @DepartmentIdentifier";

            var sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@UserIdentifier", user),
                new SqlParameter("@ProfileStandardIdentifier", profile ?? (object)DBNull.Value),
                new SqlParameter("@DepartmentIdentifier", department ?? (object)DBNull.Value)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<UserCompetencySummary.StatusSummary>(query, sqlParameters)
                    .ToArray();
        }

        #endregion

        public class ExecutiveSummaryOnCompetencyStatus
        {
            public DateTimeOffset AsAt { get; set; }
            public string DivisionName { get; set; }
            public string DepartmentName { get; set; }
            public string TagCriticality { get; set; }
            public int SumCP { get; set; }
            public int SumEX { get; set; }
            public int SumNC { get; set; }
            public int SumNA { get; set; }
            public int SumNT { get; set; }
            public int SumSA { get; set; }
            public int SumSV { get; set; }
            public int SumVA { get; set; }
            public int SumVN { get; set; }
            public int SumRQ { get; set; }
            public decimal? AvgProgress { get; set; }
            public decimal? AvgScore { get; set; }
        }

        public static ExecutiveSummaryOnCompetencyStatus[] SelectExecutiveSummaryOnCompetencyStatus(ExecutiveSummaryOnCompetencyStatusFilter filter)
        {
            var query = $@"
SELECT
           Stat.AsAt
         , Div.DivisionName
         , Dept.DepartmentName
         , Stat.TagCriticality
         , SUM(Stat.CountCP) AS SumCP
         , SUM(Stat.CountEX) AS SumEX
         , SUM(Stat.CountNC) AS SumNC
         , SUM(Stat.CountNA) AS SumNA
         , SUM(Stat.CountNT) AS SumNT
         , SUM(Stat.CountSA) AS SumSA
         , SUM(Stat.CountSV) AS SumSV
         , SUM(Stat.CountVA) AS SumVA
         , SUM(Stat.CountVN) AS SumVN
         , SUM(Stat.CountRQ) AS SumRQ
         , ROUND(100 * AVG(Stat.Score), 0) AS AvgScore
         , CASE WHEN SUM(Stat.CountRQ) = 0 THEN 0
           ELSE ROUND(100 * SUM(Stat.CountVA + Stat.CountVN) / CAST(SUM(Stat.CountRQ) AS DECIMAL), 0)
           END AS AvgProgress
FROM
           custom_cmds.TUserStatus AS Stat
INNER JOIN identities.Department   AS Dept
           ON Dept.DepartmentIdentifier = Stat.DepartmentIdentifier

LEFT JOIN  identities.Division     AS Div
           ON Div.DivisionIdentifier = Dept.DivisionIdentifier
WHERE
           {GetExecutiveSummaryOnCompetencyStatusParameters(filter)}
GROUP BY
           Stat.AsAt
         , Div.DivisionName
         , Dept.DepartmentName
         , Stat.TagCriticality
ORDER BY
           Stat.AsAt DESC
         , Div.DivisionName
         , Dept.DepartmentName
         , Stat.TagCriticality;";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<ExecutiveSummaryOnCompetencyStatus>(
                        query,
                        CreateExecutiveSummaryOnCompetencyStatusSqlParameters(filter))
                    .ToArray();
            }
        }

        private static string GetExecutiveSummaryOnCompetencyStatusParameters(ExecutiveSummaryOnCompetencyStatusFilter filter)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Stat.OrganizationIdentifier = @OrganizationIdentifier");
            builder.AppendLine("AND Dept.OrganizationIdentifier = @OrganizationIdentifier");
            builder.AppendLine("AND Stat.ListDomain = 'Standard'");
            builder.AppendLine("AND Stat.ListFolder = 'Competency'");

            if (filter.AsAt != null)
            {
                if (filter.AsAt.Since.HasValue)
                    builder.AppendLine("AND Stat.AsAt >= @AsAtSince");

                if (filter.AsAt.Before.HasValue)
                    builder.AppendLine("AND Stat.AsAt < @AsAtBefore");
            }

            if (filter.DivisionIdentifier.HasValue)
                builder.AppendLine("AND Div.DivisionIdentifier = @DivisionIdentifier");

            if (filter.DepartmentIdentifier.HasValue)
                builder.AppendLine("AND Dept.DepartmentIdentifier = @DepartmentIdentifier");

            if (filter.Criticality.HasValue())
                builder.AppendLine("AND Stat.TagCriticality = @Criticality");

            return builder.ToString();
        }

        private static SqlParameter[] CreateExecutiveSummaryOnCompetencyStatusSqlParameters(ExecutiveSummaryOnCompetencyStatusFilter filter)
        {
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@OrganizationIdentifier", filter.OrganizationIdentifier),
            };

            if (filter.AsAt != null)
            {
                if (filter.AsAt.Since.HasValue)
                    sqlParameters.Add(new SqlParameter("@AsAtSince", filter.AsAt.Since.Value));

                if (filter.AsAt.Before.HasValue)
                    sqlParameters.Add(new SqlParameter("@AsAtBefore", filter.AsAt.Before.Value));
            }

            if (filter.DivisionIdentifier.HasValue)
                sqlParameters.Add(new SqlParameter("@DivisionIdentifier", filter.DivisionIdentifier.Value));

            if (filter.DepartmentIdentifier.HasValue)
                sqlParameters.Add(new SqlParameter("@DepartmentIdentifier", filter.DepartmentIdentifier.Value));

            if (filter.Criticality.HasValue())
                sqlParameters.Add(new SqlParameter("@Criticality", filter.Criticality));

            return sqlParameters.ToArray();
        }

        public class ExecutiveSummaryOnAchievementStatus
        {
            public DateTimeOffset AsAt { get; set; }
            public string DepartmentName { get; set; }
            public string AchievementType { get; set; }
            public int SumCP { get; set; }
            public int SumEX { get; set; }
            public int SumNC { get; set; }
            public int SumNA { get; set; }
            public int SumNT { get; set; }
            public int SumSA { get; set; }
            public int SumSV { get; set; }
            public int SumVA { get; set; }
            public int SumVN { get; set; }
            public int SumRQ { get; set; }
            public decimal? AvgProgress { get; set; }
            public decimal? AvgScore { get; set; }
        }

        public static ExecutiveSummaryOnAchievementStatus[] SelectExecutiveSummaryOnAchievementStatus(ExecutiveSummaryOnAchievementStatusFilter filter)
        {
            var query = $@"
SELECT
           Stat.AsAt
         , Dept.DepartmentName
         , Stat.ItemName AS  AchievementType
         , SUM(Stat.CountCP) AS SumCP
         , SUM(Stat.CountEX) AS SumEX
         , SUM(Stat.CountNC) AS SumNC
         , SUM(Stat.CountNA) AS SumNA
         , SUM(Stat.CountNT) AS SumNT
         , SUM(Stat.CountSA) AS SumSA
         , SUM(Stat.CountSV) AS SumSV
         , SUM(Stat.CountVA) AS SumVA
         , SUM(Stat.CountVN) AS SumVN
         , SUM(Stat.CountRQ) AS SumRQ
         , ROUND(100 * AVG(Stat.Score),0) AS AvgScore
         , CASE WHEN SUM(Stat.CountRQ) = 0 THEN 0
           -- ELSE ROUND(100 * SUM(Stat.CountVA + Stat.CountVN) / CAST(SUM(Stat.CountRQ) AS DECIMAL), 0)
           ELSE ROUND(100 * SUM(Stat.CountCP) / CAST(SUM(Stat.CountRQ) AS DECIMAL), 0)
           END AS AvgProgress
FROM
           custom_cmds.TUserStatus AS Stat
INNER JOIN identities.Department   AS Dept
           ON Dept.DepartmentIdentifier = Stat.DepartmentIdentifier
WHERE
          {GetExecutiveSummaryOnAchievementStatusParameters(filter)}
GROUP BY
           Stat.AsAt
         , Dept.DepartmentName
         , Stat.ItemName
ORDER BY
           Stat.AsAt DESC
         , Dept.DepartmentName
         , Stat.ItemName;";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<ExecutiveSummaryOnAchievementStatus>(
                        query,
                        CreateExecutiveSummaryOnAchievementStatusSqlParameters(filter))
                    .ToArray();
            }
        }

        private static string GetExecutiveSummaryOnAchievementStatusParameters(ExecutiveSummaryOnAchievementStatusFilter filter)
        {
            var builder = new StringBuilder();

            builder.AppendLine("Stat.OrganizationIdentifier = @OrganizationIdentifier");
            builder.AppendLine("AND Dept.OrganizationIdentifier = @OrganizationIdentifier");
            builder.AppendLine("AND Stat.ListDomain = 'Resource'");

            if (filter.AsAt != null)
            {
                if (filter.AsAt.Since.HasValue)
                    builder.AppendLine("AND Stat.AsAt >= @AsAtSince");

                if (filter.AsAt.Before.HasValue)
                    builder.AppendLine("AND Stat.AsAt < @AsAtBefore");
            }

            if (filter.DepartmentIdentifier.HasValue)
                builder.AppendLine("AND Dept.DepartmentIdentifier = @DepartmentIdentifier");

            if (filter.AchievementType.HasValue())
                builder.AppendLine("AND Stat.ItemName = @AchievementType");

            return builder.ToString();
        }

        private static SqlParameter[] CreateExecutiveSummaryOnAchievementStatusSqlParameters(ExecutiveSummaryOnAchievementStatusFilter filter)
        {
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@OrganizationIdentifier", filter.OrganizationIdentifier),
            };

            if (filter.AsAt != null)
            {
                if (filter.AsAt.Since.HasValue)
                    sqlParameters.Add(new SqlParameter("@AsAtSince", filter.AsAt.Since.Value));

                if (filter.AsAt.Before.HasValue)
                    sqlParameters.Add(new SqlParameter("@AsAtBefore", filter.AsAt.Before.Value));
            }

            if (filter.DepartmentIdentifier.HasValue)
                sqlParameters.Add(new SqlParameter("@DepartmentIdentifier", filter.DepartmentIdentifier.Value));

            if (filter.AchievementType.HasValue())
                sqlParameters.Add(new SqlParameter("@AchievementType", filter.AchievementType));

            return sqlParameters.ToArray();
        }
    }
}
