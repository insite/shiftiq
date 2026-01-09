using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Commands;

using InSite.Application.StandardValidations.Write;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class UserCompetencyRepository
    {
        #region Intialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        #region INSERT

        public static void UpdateEmployeeCompetencies(Guid userId)
        {
            Guid[] competencies;

            using (var db = new InternalDbContext())
            {
                competencies = db.UserProfiles
                    .Join(
                        db.ProfileCompetencies,
                        up => up.ProfileStandardIdentifier,
                        uc => uc.ProfileStandardIdentifier,
                        (up, pc) => new
                        {
                            UserProfile = up,
                            ProfileCompetency = pc
                        })
                    .Where(x => x.UserProfile.UserIdentifier == userId
                             && !db.UserCompetencies
                                .Where(uc => uc.UserIdentifier == userId)
                                .Select(uc => uc.CompetencyStandardIdentifier)
                                .Contains(x.ProfileCompetency.CompetencyStandardIdentifier))
                    .Select(x => x.ProfileCompetency.CompetencyStandardIdentifier)
                    .Distinct()
                    .ToArray();
            }

            foreach (var competencyId in competencies)
                _sendCommand(new CreateStandardValidation(UniqueIdentifier.Create(), competencyId, userId));
        }

        public static void SubmitForValidation(Guid user)
        {
            Guid[] validationIds;

            using (var db = new InternalDbContext())
            {
                validationIds = db.QStandardValidations
                    .Where(x => x.UserIdentifier == user && x.ValidationDate == null && (x.ValidationStatus == ValidationStatuses.SelfAssessed || x.ValidationStatus == ValidationStatuses.NotApplicable))
                    .Select(x => x.StandardValidationIdentifier)
                    .ToArray();
            }

            foreach (var validationId in validationIds)
                _sendCommand(new SubmitForValidationStandardValidation(validationId, UniqueIdentifier.Create()));
        }

        public static void AddNewCompetencies(Guid profileStandardIdentifier, IEnumerable<Guid> users)
        {
            Guid[] competencies;
            HashSet<(Guid, Guid)> validations;

            using (var db = new InternalDbContext())
            {
                competencies = db.StandardContainments
                    .Where(x => x.ParentStandardIdentifier == profileStandardIdentifier)
                    .Select(x => x.ChildStandardIdentifier)
                    .ToArray();

                validations = db.StandardValidations
                    .Where(x => users.Contains(x.UserIdentifier))
                    .Select(x => new { x.StandardIdentifier, x.UserIdentifier })
                    .AsEnumerable()
                    .Select(x => (x.StandardIdentifier, x.UserIdentifier))
                    .ToHashSet();
            }

            foreach (var userId in users)
            {
                foreach (var competencyId in competencies)
                {
                    if (validations.Contains((competencyId, userId)))
                        continue;

                    _sendCommand(new CreateStandardValidation(UniqueIdentifier.Create(), competencyId, userId));
                }
            }
        }

        #endregion

        #region UPDATE

        private const string ExpiredComment = "The expiry date is manually forced to today's date.";

        public static void ExpireForPerson(Guid userId, Guid competencyId)
        {
            Guid? validationId;
            using (var db = new InternalDbContext())
            {
                validationId = db.QStandardValidations
                    .Where(x => x.UserIdentifier == userId && x.StandardIdentifier == competencyId)
                    .Select(x => (Guid?)x.StandardValidationIdentifier)
                    .FirstOrDefault();
            }

            if (validationId.HasValue)
                _sendCommand(new ExpireStandardValidation(validationId.Value, UniqueIdentifier.Create(), ExpiredComment));
        }

        public static void ExpireForDepartment(Guid departmentId, Guid competencyId)
        {
            Guid[] validationsIds;

            using (var db = new InternalDbContext())
            {
                validationsIds = db.QStandardValidations
                    .Where(v => v.StandardIdentifier == competencyId
                        && (v.Expired == null || v.ValidationStatus != ValidationStatuses.Expired || v.SelfAssessmentStatus != ValidationStatuses.Expired)
                        && db.Memberships
                            .Where(m => m.GroupIdentifier == departmentId && m.MembershipType == "Department")
                            .Select(m => m.UserIdentifier)
                            .Contains(v.UserIdentifier))
                    .Select(x => x.StandardValidationIdentifier)
                    .ToArray();
            }

            foreach (var validationId in validationsIds)
                _sendCommand(new ExpireStandardValidation(validationId, UniqueIdentifier.Create(), ExpiredComment));
        }

        public static void ExpireForCompany(Guid organizationId, Guid competencyId)
        {
            Guid[] validationsIds;

            using (var db = new InternalDbContext())
            {
                validationsIds = db.QStandardValidations
                    .Where(v => v.StandardIdentifier == competencyId
                        && (v.Expired == null || v.ValidationStatus != ValidationStatuses.Expired || v.SelfAssessmentStatus != ValidationStatuses.Expired)
                        && db.Memberships
                            .Where(m => m.Group.GroupType == "Department" && m.Group.OrganizationIdentifier == organizationId)
                            .Select(m => m.UserIdentifier)
                            .Contains(v.UserIdentifier))
                    .Select(x => x.StandardValidationIdentifier)
                    .ToArray();
            }

            foreach (var validationId in validationsIds)
                _sendCommand(new ExpireStandardValidation(validationId, UniqueIdentifier.Create(), ExpiredComment));
        }

        #endregion

        #region SELECT

        public static UserCompetency Select(Guid userKey, Guid competencyStandardIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.UserCompetencies.FirstOrDefault(x => x.UserIdentifier == userKey && x.CompetencyStandardIdentifier == competencyStandardIdentifier);
        }

        public static DataTable SelectCertifications(Guid userKey, Guid organizationId, Guid? profileStandardIdentifier)
        {
            var query = $@"
WITH Profiles AS (
    SELECT DISTINCT
        @UserIdentifier AS UserIdentifier
       ,@OrganizationIdentifier AS OrganizationIdentifier
       ,P.ProfileStandardIdentifier
       ,P.ProfileNumber
       ,P.ProfileTitle
       ,P.CertificationHoursPercentCore
       ,P.CertificationHoursPercentNonCore
       ,[cert].DateRequested
       ,[cert].DateGranted
       ,[cert].DateSubmitted
    FROM
        custom_cmds.[Profile] AS P
        INNER JOIN custom_cmds.VCmdsProfileOrganization AS CP
            ON CP.ProfileStandardIdentifier = P.ProfileStandardIdentifier
        LEFT JOIN  custom_cmds.ProfileCertification AS [cert]
            ON [cert].ProfileStandardIdentifier = P.ProfileStandardIdentifier
               AND [cert].UserIdentifier = @UserIdentifier
    WHERE
        P.IsCertificateEnabled = 1
        AND CP.OrganizationIdentifier = @OrganizationIdentifier
        AND P.ProfileStandardIdentifier IN (
            SELECT
                dp.ProfileStandardIdentifier
            FROM
                contacts.Membership AS m
                INNER JOIN identities.Department AS d
                    ON d.DepartmentIdentifier = m.GroupIdentifier
                INNER JOIN custom_cmds.DepartmentProfile AS dp
                    ON dp.DepartmentIdentifier = m.GroupIdentifier
            WHERE
                m.UserIdentifier = @UserIdentifier
                AND d.OrganizationIdentifier = @OrganizationIdentifier
        )
)
SELECT DISTINCT
    UserIdentifier
   ,ProfileStandardIdentifier
   ,ProfileNumber
   ,ProfileTitle
   ,CAST(ISNULL(CompletedHoursHelper.CoreHours, 0) AS DECIMAL(7, 2))                                         AS CoreHours
   ,CAST(ISNULL(CompletedHoursHelper.NonCoreHours, 0) AS DECIMAL(7, 2))                                      AS NonCoreHours
   ,TotalHoursHelper.CoreHours                                                                               AS CoreHoursTotal
   ,TotalHoursHelper.NonCoreHours                                                                            AS NonCoreHoursTotal
   ,CAST(ISNULL(CertificationHoursPercentCore * TotalHoursHelper.CoreHours / 100, 0) AS DECIMAL(7, 2))       AS CoreHoursRequired
   ,CAST(ISNULL(CertificationHoursPercentNonCore * TotalHoursHelper.NonCoreHours / 100, 0) AS DECIMAL(7, 2)) AS NonCoreHoursRequired
   ,DateRequested
   ,DateGranted
   ,DateSubmitted
FROM
    Profiles
    OUTER APPLY (
        SELECT
            SUM(ISNULL(Q.CertificationHoursCore, 0)) AS CoreHours
           ,SUM(ISNULL(Q.CertificationHoursNonCore, 0)) AS NonCoreHours
        FROM
            custom_cmds.ProfileCompetency AS Q
        WHERE
            Q.ProfileStandardIdentifier = Profiles.ProfileStandardIdentifier
            AND Q.CompetencyStandardIdentifier NOT IN (
                SELECT DISTINCT
                    S.CompetencyStandardIdentifier
                FROM
                    custom_cmds.Competency AS S
                    INNER JOIN custom_cmds.UserCompetency AS C
                        ON S.CompetencyStandardIdentifier = C.CompetencyStandardIdentifier
                WHERE
                    S.IsDeleted = 1
            )
    ) AS TotalHoursHelper
    OUTER APPLY (
        SELECT
            SUM(ISNULL(Q.CertificationHoursCore, 0))    AS CoreHours
           ,SUM(ISNULL(Q.CertificationHoursNonCore, 0)) AS NonCoreHours
        FROM
            custom_cmds.Competency AS S
            INNER JOIN custom_cmds.UserCompetency AS C
                ON S.CompetencyStandardIdentifier = C.CompetencyStandardIdentifier
            INNER JOIN custom_cmds.ProfileCompetency AS Q
                ON S.CompetencyStandardIdentifier = Q.CompetencyStandardIdentifier
        WHERE
            S.IsDeleted = 0
            AND C.ValidationStatus IN ('Validated', 'Expired')
            AND C.UserIdentifier = Profiles.UserIdentifier
            AND Q.ProfileStandardIdentifier = Profiles.ProfileStandardIdentifier
    ) AS CompletedHoursHelper
WHERE
    TotalHoursHelper.CoreHours > 0
    OR TotalHoursHelper.NonCoreHours > 0
ORDER BY
    ProfileTitle;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
            };

            if (profileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static DataTable SelectCompetenciesThatNeedToBeExpired(OrganizationExpirationType expirationType, DateTimeOffset now)
        {
            var query = expirationType == OrganizationExpirationType.Interval
                ? "EXEC custom_cmds.SelectCompetenciesExpiringByInterval @Now"
                : "EXEC custom_cmds.SelectCompetenciesExpiringByDate @Now";

            return DatabaseHelper.CreateDataTable(query, 60 * 5, new SqlParameter("Now", now));
        }

        public static ListItem[] SelectAchievementTypes(Dictionary<string, string> translator)
        {
            var list = new List<ListItem>();

            const string query = "select distinct ItemNumber, ItemName from custom_cmds.QUserStatus with (nolock) order by ItemNumber";

            var table = DatabaseHelper.CreateDataTable(query);

            foreach (DataRow row in table.Rows)
            {
                var name = (string)row["ItemName"];

                var item = new ListItem
                {
                    Text = translator.GetOrDefault(name, name),
                    Value = row["ItemNumber"].ToString()
                };

                list.Add(item);
            }

            return list.OrderBy(x => x.Text).ToArray();
        }

        public static DataTable SelectCertificationMissingCompetencies(Guid profileStandardIdentifier, Guid userKey, Guid organizationId)
        {
            const string query = @"
                SELECT
                    c.Number AS CompetencyNumber,
                    c.Summary AS CompetencySummary,
                    CASE
                        WHEN cc.CompetencyStandardIdentifier IS NOT NULL THEN ec.ValidationStatus
                        ELSE 'Not Assigned'
                    END AS ValidationStatus,
                    pc.CertificationHoursCore,
                    pc.CertificationHoursNonCore
                  FROM custom_cmds.ProfileCompetency pc
                  INNER JOIN custom_cmds.Competency c
                    ON c.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                  LEFT JOIN custom_cmds.UserCompetency ec
                    ON ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                      AND ec.UserIdentifier = @UserIdentifier
                  LEFT JOIN custom_cmds.VCmdsCompetencyOrganization AS cc
                    ON cc.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                      AND cc.OrganizationIdentifier = @OrganizationIdentifier
                  WHERE c.IsDeleted = 0
                    AND pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
                    AND (ec.ValidationStatus IS NULL OR ec.ValidationStatus NOT IN ('Validated', 'Expired'))
                    AND (pc.CertificationHoursCore IS NOT NULL OR pc.CertificationHoursNonCore IS NOT NULL)
                  ORDER BY c.Number
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier),
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
                );
        }

        public static DataTable SelectItemCountForValidator(Guid validatorUserIdentifier, Guid organizationId, int? pageIndex, int? pageSize)
        {
            const string queryRole = @"
        SELECT TOP 1 1
            FROM identities.[User]
            WHERE UserIdentifier IN (SELECT UserIdentifier FROM custom_cmds.UserRole WHERE GroupName = 'CMDS Super Validators')
            AND UserIdentifier = @ValidatorUserIdentifier
";

            const string subqueryValidatorCompetencies = @"
    AND UserCompetency.CompetencyStandardIdentifier IN (
            SELECT CompetencyStandardIdentifier
                FROM custom_cmds.UserCompetency x
                WHERE x.UserIdentifier = @ValidatorUserIdentifier
                AND ( (x.ValidationStatus IN ('Validated', 'Expired'))
                        OR
                        (x.ValidationStatus = 'Not Applicable' and UserCompetency.SelfAssessmentStatus = 'Not Applicable')
                    )
                AND ValidationDate IS NOT NULL
            )
";

            const string query = @"
SELECT
    p.UserIdentifier,
    p.FullName,
    COUNT(DISTINCT Competency.CompetencyStandardIdentifier) AS ItemCount
FROM
    identities.UserConnection
    INNER JOIN identities.[User] p ON p.UserIdentifier = UserConnection.ToUserIdentifier
    INNER JOIN custom_cmds.UserCompetency ON UserCompetency.UserIdentifier = p.UserIdentifier
    INNER JOIN custom_cmds.Competency ON Competency.CompetencyStandardIdentifier = UserCompetency.CompetencyStandardIdentifier
    INNER JOIN standards.DepartmentProfileUser ON DepartmentProfileUser.UserIdentifier = UserCompetency.UserIdentifier
    INNER JOIN custom_cmds.DepartmentProfileCompetency ON DepartmentProfileCompetency.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier
                                                   AND DepartmentProfileCompetency.ProfileStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
                                                   AND DepartmentProfileCompetency.CompetencyStandardIdentifier = UserCompetency.CompetencyStandardIdentifier

    INNER JOIN standards.StandardContainment AS ProfileCompetency ON ProfileCompetency.ParentStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
                                                            AND ProfileCompetency.ChildStandardIdentifier = UserCompetency.CompetencyStandardIdentifier

    INNER JOIN identities.Department ON Department.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier

WHERE
    UserConnection.FromUserIdentifier = @ValidatorUserIdentifier
    AND UserConnection.IsValidator = 1
    AND department.OrganizationIdentifier = @OrganizationIdentifier
    AND UserCompetency.UserIdentifier IN (
        SELECT DISTINCT
            m.UserIdentifier
        FROM
            contacts.Membership AS m
            INNER JOIN identities.Department ON Department.DepartmentIdentifier = m.GroupIdentifier
        WHERE
            Department.OrganizationIdentifier = @OrganizationIdentifier
    )
    AND Competency.IsDeleted = 0
    AND UserCompetency.ValidationStatus IN ('Submitted for Validation')
{0}
GROUP BY
    p.UserIdentifier,
    p.FullName
";

            var curQuery = query;

            if (pageIndex.HasValue)
            {
                curQuery += @"
ORDER BY p.FullName
OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY
";
            }

            bool hasValidatorRole;

            using (var db = new InternalDbContext())
            {
                hasValidatorRole = db.Database.SqlQuery<int?>(queryRole, new SqlParameter("ValidatorUserIdentifier", validatorUserIdentifier)).FirstOrDefault() != null;
            }

            curQuery = hasValidatorRole
                ? string.Format(curQuery, string.Empty)
                : string.Format(curQuery, subqueryValidatorCompetencies);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("ValidatorUserIdentifier", validatorUserIdentifier)
            };

            if (pageIndex.HasValue)
            {
                parameters.Add(new SqlParameter("PageIndex", pageIndex));
                parameters.Add(new SqlParameter("PageSize", pageSize));
            }

            return DatabaseHelper.CreateDataTable(curQuery, 5 * 60, parameters.ToArray());
        }

        public static DataTable SelectExpiringCompetencies(UserProfileKey key)
        {
            const string query = @"
SELECT
    ec.UserIdentifier
   ,ec.ValidatorUserIdentifier
   ,ec.CompetencyStandardIdentifier
   ,ec.ValidationStatus
   ,ec.IsValidated
   ,ec.ValidationDate
   ,ec.SelfAssessmentDate
   ,ec.SelfAssessmentStatus
   ,ISNULL(ec.ExpirationDate, CASE WHEN cs.ValidForUnit = 'Years' THEN DATEADD(year, cs.ValidForCount, ec.ValidationDate)
                                   WHEN cs.ValidForUnit = 'Months' THEN DATEADD(month, cs.ValidForCount, ec.ValidationDate)
                                   ELSE NULL
                              END) AS ExpirationDate
   ,c.Summary AS SkillName
   ,c.NumberOld
   ,c.Number
FROM
    custom_cmds.UserCompetency ec
    INNER JOIN custom_cmds.Competency c
      ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.ProfileCompetency pc
      ON pc.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.UserProfile ep
      ON ep.UserIdentifier = ec.UserIdentifier
        AND ep.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
    INNER JOIN custom_cmds.DepartmentProfileCompetency cs
        ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
          AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
          AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
WHERE
    ( @ExpirationDate >= ISNULL(ec.ExpirationDate, CASE WHEN cs.ValidForUnit = 'Years' THEN DATEADD(year, cs.ValidForCount, ec.ValidationDate)
                                                        WHEN cs.ValidForUnit = 'Months' THEN DATEADD(month, cs.ValidForCount, ec.ValidationDate)
                                                        ELSE NULL
                                                   END) OR ValidationStatus = 'Expired' )
    AND ec.UserIdentifier = @UserIdentifier
    AND c.IsDeleted = 0 
    AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
    AND ep.DepartmentIdentifier = @DepartmentIdentifier
ORDER BY
    c.Number";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", key.UserIdentifier),
                new SqlParameter("ProfileStandardIdentifier", key.ProfileStandardIdentifier),
                new SqlParameter("DepartmentIdentifier", key.DepartmentIdentifier),
                new SqlParameter("ExpirationDate", DateTime.UtcNow.Date)
                );
        }

        public static DataTable SelectStatusCountsForManager(Guid managerUserIdentifier, Guid organizationId)
        {
            const string query = @"
SELECT
    COUNT(*) AS [Count]
   ,UserCompetency.ValidationStatus
FROM
    identities.Department
    INNER JOIN custom_cmds.DepartmentProfileCompetency ON DepartmentProfileCompetency.DepartmentIdentifier = Department.DepartmentIdentifier
    INNER JOIN standards.DepartmentProfileUser ON DepartmentProfileUser.ProfileStandardIdentifier = DepartmentProfileCompetency.ProfileStandardIdentifier
                                                AND DepartmentProfileUser.DepartmentIdentifier = DepartmentProfileCompetency.DepartmentIdentifier
    INNER JOIN custom_cmds.UserCompetency ON UserCompetency.UserIdentifier = DepartmentProfileUser.UserIdentifier
                                                AND UserCompetency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.ProfileCompetency ON ProfileCompetency.ProfileStandardIdentifier = DepartmentProfileCompetency.ProfileStandardIdentifier
                                                            AND ProfileCompetency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.Competency ON Competency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
WHERE
    DepartmentProfileUser.IsPrimary = 1
    AND Competency.IsDeleted = 0
    AND Department.OrganizationIdentifier = @OrganizationIdentifier
    AND UserCompetency.ValidationStatus IS NOT NULL
    AND UserCompetency.UserIdentifier IN (
        SELECT ToUserIdentifier
            FROM identities.UserConnection
            WHERE FromUserIdentifier = @ManagerUserIdentifier
            AND (IsManager = 1 OR IsSupervisor = 1)
    )
GROUP BY
    UserCompetency.ValidationStatus
";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("ManagerUserIdentifier", managerUserIdentifier),
                new SqlParameter("OrganizationIdentifier", organizationId)
                );
        }

        public static DataTable SelectStatusCountsForEmployeePrimaryProfile(Guid userKey, Guid organizationId)
        {
            const string query = @"
SELECT
    COUNT(*) AS [Count]
   ,ec.ValidationStatus
FROM
    custom_cmds.UserCompetency ec
    INNER JOIN custom_cmds.Competency c ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.ProfileCompetency pc ON pc.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.Employment e ON e.UserIdentifier = ec.UserIdentifier
                                       AND e.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
    INNER JOIN custom_cmds.DepartmentProfileCompetency cs ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                                                       AND cs.DepartmentIdentifier = e.DepartmentIdentifier
                                                       AND cs.ProfileStandardIdentifier = e.ProfileStandardIdentifier
WHERE
    c.IsDeleted = 0
    AND ec.ValidationStatus IS NOT NULL
    AND ec.UserIdentifier = @UserIdentifier
    AND e.OrganizationIdentifier = @OrganizationIdentifier
GROUP BY
    ec.ValidationStatus
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
                );
        }

        public static DataTable SelectStatusCountsForEmployeeProfile(Guid userKey, Guid profileStandardIdentifier, Guid department)
        {
            const string query = @"
                SELECT
                    COUNT(*) AS [Count],
                    ec.ValidationStatus
                  FROM custom_cmds.UserCompetency ec
                  INNER JOIN custom_cmds.Competency c
                    ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.ProfileCompetency pc
                    ON pc.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.UserProfile ep
                    ON ep.UserIdentifier = ec.UserIdentifier
                      AND ep.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
                  INNER JOIN custom_cmds.DepartmentProfileCompetency cs
                    ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                      AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
                      AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  WHERE
                    c.IsDeleted = 0
                    AND ec.ValidationStatus IS NOT NULL
                    AND ep.UserIdentifier = @UserIdentifier
                    AND ep.DepartmentIdentifier = @DepartmentIdentifier
                    AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
                  GROUP BY ec.ValidationStatus
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier),
                new SqlParameter("DepartmentIdentifier", department)
                );
        }

        public static DataTable SelectStatusCountsForComplianceProfiles(Guid userKey, Guid organizationId)
        {
            const string query = @"
DECLARE @PrimaryProfileIdentifier UNIQUEIDENTIFIER;

SELECT @PrimaryProfileIdentifier = ProfileStandardIdentifier
FROM custom_cmds.Employment
WHERE UserIdentifier = @UserIdentifier
      AND OrganizationIdentifier = @OrganizationIdentifier;

SELECT COUNT(DISTINCT c.CompetencyStandardIdentifier) AS [Count],
       ec.ValidationStatus
FROM custom_cmds.UserCompetency ec
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.ProfileCompetency pc
        ON pc.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.UserProfile ep
        ON ep.UserIdentifier = ec.UserIdentifier
           AND ep.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
           AND
           (
               ep.IsComplianceRequired = 1
               OR ep.ProfileStandardIdentifier = @PrimaryProfileIdentifier
           )
    INNER JOIN identities.Department AS departments
        ON departments.DepartmentIdentifier = ep.DepartmentIdentifier
WHERE c.IsDeleted = 0
      AND ec.ValidationStatus IS NOT NULL
      AND ep.UserIdentifier = @UserIdentifier
      AND departments.OrganizationIdentifier = @OrganizationIdentifier
GROUP BY ec.ValidationStatus
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
                );
        }

        #endregion

        #region Filtering

        public static DataTable SelectStatusCountsForEmployee(Guid userKey, Guid organizationId)
        {
            var filter = new EmployeeCompetencyFilter { UserIdentifier = userKey, OrganizationIdentifier = organizationId };
            var join = CreateJoinForSelectSearchResults(filter);
            var where = CreateWhereForSelectSearchResults(filter, null, null);
            var query = $@"
SELECT
    COUNT(*) AS [Count],
    ec.ValidationStatus
FROM
    custom_cmds.UserCompetency ec
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
{join}
{where}
    AND ec.ValidationStatus IS NOT NULL
GROUP BY
    ec.ValidationStatus";

            return DatabaseHelper.CreateDataTable(query, 2 * 60, GetParametersForSelectSearchResults(filter, null, null).ToArray());
        }

        public static DataTable SelectComplianceSummaryX(Guid userKey, int profileStandardIdentifier, int department)
        {
            const string query = @"
                SELECT
                    COUNT(*) AS Total,
                    COUNT(CASE WHEN ec.IsValidated = 1 AND ec.ValidationStatus IN ('Validated','Not Applicable') THEN 1 ELSE NULL END) AS TotalValidated,
                    COUNT(CASE WHEN ec.ValidationStatus = 'Submitted for Validation' THEN 1 ELSE NULL END) AS TotalSubmitted,
                    COUNT(CASE WHEN cs.Criticality = 'Critical' THEN 1 ELSE NULL END) AS Critical,
                    COUNT(CASE WHEN ec.IsValidated = 1 AND ec.ValidationStatus IN ('Validated','Not Applicable') AND cs.Criticality = 'Critical' THEN 1 ELSE NULL END) AS CriticalValidated,
                    COUNT(CASE WHEN ec.ValidationStatus = 'Submitted for Validation' AND cs.Criticality = 'Critical' THEN 1 ELSE NULL END) AS CriticalSubmitted,
                    @ProfileStandardIdentifier AS ProfileStandardIdentifier
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.ProfileCompetency pc
                    ON pc.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN custom_cmds.UserCompetency ec
                    ON ec.UserIdentifier = ep.UserIdentifier
                      AND ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.Competency c
                    ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.DepartmentProfileCompetency cs
                    ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                      AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
                      AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  WHERE
                    c.IsDeleted = 0
                    AND ep.UserIdentifier = @UserIdentifier
                    AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
                    AND ep.DepartmentIdentifier = @DepartmentIdentifier
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier),
                new SqlParameter("DepartmentIdentifier", department)
                );
        }

        public static UserStatusHome SelectComplianceSummary(Guid user, Guid organization, bool primaryOnly)
        {
            const string query = @"
                EXEC custom_cmds.QSelectUserStatusHome @UserIdentifier, @OrganizationIdentifier, @PrimaryOnly
                ";

            var table = DatabaseHelper.CreateDataTable(query
                , new SqlParameter("UserIdentifier", user)
                , new SqlParameter("OrganizationIdentifier", organization)
                , new SqlParameter("PrimaryOnly", primaryOnly ? 1 : 0)
                );

            var home = new UserStatusHome
            {
                Critical = GetValue("Critical"),
                CriticalValidated = GetValue("Critical Validated"),
                CriticalSubmitted = GetValue("Critical Submitted"),
                NonCritical = GetValue("Non-Critical"),
                NonCriticalValidated = GetValue("Non-Critical Validated"),
                NonCriticalSubmitted = GetValue("Non-Critical Submitted")
            };

            return home;

            int GetValue(string name)
            {
                var rows = table.Select($"Name='{name}'");
                if (rows.Length == 1)
                    return (int)rows[0]["Value"];
                return 0;
            }
        }

        public static DataTable SelectComplianceSummaryX(Guid userKey, Guid organizationId)
        {
            const string query = @"
                DECLARE @PrimaryProfileIdentifier INT

                SELECT @PrimaryProfileIdentifier = ProfileStandardIdentifier FROM custom_cmds.Employment WHERE UserIdentifier = @UserIdentifier AND OrganizationIdentifier = @OrganizationIdentifier

                SELECT
                    COUNT(DISTINCT c.CompetencyStandardIdentifier) AS Total,
                    COUNT(DISTINCT CASE WHEN ec.IsValidated = 1 AND ec.ValidationStatus IN ('Validated','Not Applicable') THEN c.CompetencyStandardIdentifier ELSE NULL END) AS TotalValidated,
                    COUNT(DISTINCT CASE WHEN ec.ValidationStatus = 'Submitted for Validation' THEN c.CompetencyStandardIdentifier ELSE NULL END) AS TotalSubmitted,
                    COUNT(DISTINCT CASE WHEN cs.Criticality = 'Critical' THEN c.CompetencyStandardIdentifier ELSE NULL END) AS Critical,
                    COUNT(DISTINCT CASE WHEN ec.IsValidated = 1 AND ec.ValidationStatus IN ('Validated','Not Applicable') AND cs.Criticality = 'Critical' THEN c.CompetencyStandardIdentifier ELSE NULL END) AS CriticalValidated,
                    COUNT(DISTINCT CASE WHEN ec.ValidationStatus = 'Submitted for Validation' AND cs.Criticality = 'Critical' THEN c.CompetencyStandardIdentifier ELSE NULL END) AS CriticalSubmitted,
                    NULL AS ProfileStandardIdentifier
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.ProfileCompetency pc
                    ON pc.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN custom_cmds.UserCompetency ec
                    ON ec.UserIdentifier = ep.UserIdentifier
                      AND ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.Competency c
                    ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
                  INNER JOIN custom_cmds.DepartmentProfileCompetency cs
                    ON cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                      AND cs.DepartmentIdentifier = ep.DepartmentIdentifier
                      AND cs.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department AS departments ON departments.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE
                    c.IsDeleted = 0
                    AND (ep.IsComplianceRequired = 1 OR ep.ProfileStandardIdentifier = @PrimaryProfileIdentifier)
                    AND ep.UserIdentifier = @UserIdentifier
                    AND departments.OrganizationIdentifier = @OrganizationIdentifier
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
                );
        }

        public static DataTable SelectSearchResults(EmployeeCompetencyFilter filter, Guid? validatorUserIdentifier, Guid? parentUserId)
        {
            var join = CreateJoinForSelectSearchResults(filter);
            var where = CreateWhereForSelectSearchResults(filter, validatorUserIdentifier, parentUserId);
            var query = $@"
SELECT
    ec.*,
    c.Summary,
    c.Number,
    c.NumberOld
FROM custom_cmds.UserCompetency ec
INNER JOIN custom_cmds.Competency c
    ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
{join}
{where}";

            return DatabaseHelper.CreateDataTable(query, 5 * 60, GetParametersForSelectSearchResults(filter, validatorUserIdentifier, parentUserId).ToArray());
        }

        public static DataTable SelectSearchResultsPaged(EmployeeCompetencyFilter filter,
            Guid? validatorUserIdentifier,
            Guid? parentUserId
            )
        {
            var join = CreateJoinForSelectSearchResults(filter);
            var where = CreateWhereForSelectSearchResults(filter, validatorUserIdentifier, parentUserId);

            var sortExpression = "Number";

            var employeeJoin = filter.UserIdentifier == null
                ? @"
                    INNER JOIN identities.[User] p
                      ON p.UserIdentifier = ec.UserIdentifier
                    "
                : null;

            var withSortExpression = sortExpression
                .Replace("PriorityName", "settings.PriorityName")
                .Replace("Category", "Classification.CategoryName")
                ;

            var query = $@"
WITH OrderedEmployeeCompetencies AS
(
    SELECT
        ec.*,
        c.Summary,
        Classification.CategoryName AS Category,
        c.Number,
        c.NumberOld,
        c.Knowledge,
        c.Skills,
        settings.ValidForUnit,
        settings.ValidForCount,
        settings.PriorityName,
        {(filter.UserIdentifier == null ? "p.FullName" : "NULL")} AS EmployeeFullName,
        ROW_NUMBER() OVER(ORDER BY {withSortExpression}) AS RowNumber
    FROM custom_cmds.UserCompetency ec
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
    {employeeJoin}
    {join}
    {where}
)
SELECT * FROM OrderedEmployeeCompetencies
    WHERE RowNumber BETWEEN @StartRow AND @EndRow
    ORDER BY {sortExpression}";

            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForSelectSearchResults(filter, validatorUserIdentifier, parentUserId);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, 5 * 60, parameters.ToArray());
        }

        public static int CountSearchResults(EmployeeCompetencyFilter filter, Guid? validatorUserIdentifier, Guid? parentUserId)
        {

            var join = CreateJoinForSelectSearchResults(filter);
            var where = CreateWhereForSelectSearchResults(filter, validatorUserIdentifier, parentUserId);
            var query = $@"
SELECT COUNT(*)
FROM custom_cmds.UserCompetency ec WITH(NOLOCK)
INNER JOIN custom_cmds.Competency c WITH(NOLOCK)
    ON c.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
{join}
{where}";

            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = 60;
                return db.Database.SqlQuery<int>(query, GetParametersForSelectSearchResults(filter, validatorUserIdentifier, parentUserId).ToArray()).FirstOrDefault();
            }
        }

        private static List<SqlParameter> GetParametersForSelectSearchResults(EmployeeCompetencyFilter filter, Guid? validatorUserIdentifier, Guid? parentUserId)
        {
            var parameters = new List<SqlParameter>();

            if (filter.UserIdentifier.HasValue)
                parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (filter.UserDepartmentIdentifier.HasValue)
                parameters.Add(new SqlParameter("EmployeeDepartmentIdentifier", filter.UserDepartmentIdentifier));

            if (filter.EmployeeDepartmentAssignment.IsNotEmpty())
                parameters.Add(new SqlParameter("EmployeeDepartmentAssignment", filter.EmployeeDepartmentAssignment));

            if (filter.ProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileStandardIdentifier", filter.ProfileStandardIdentifier));

            if (filter.Criticality.IsNotEmpty())
                parameters.Add(new SqlParameter("Criticality", filter.Criticality));

            if (filter.CategoryIdentifier.HasValue)
                parameters.Add(new SqlParameter("CategoryIdentifier", filter.CategoryIdentifier));

            if (filter.SelfAssessmentStatus.IsNotEmpty())
                parameters.Add(new SqlParameter("SelfAssessmentStatus", filter.SelfAssessmentStatus));

            if (filter.Keyword.IsNotEmpty())
                parameters.Add(new SqlParameter("Keyword", string.Format("%{0}%", filter.Keyword)));

            if (filter.Number.IsNotEmpty())
                parameters.Add(new SqlParameter("Number", string.Format("%{0}%", filter.Number)));

            if (filter.NumberOld.IsNotEmpty())
                parameters.Add(new SqlParameter("NumberOld", string.Format("%{0}%", filter.NumberOld)));

            if (validatorUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ValidatorUserIdentifier", validatorUserIdentifier));

            if (filter.ManagerUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ManagerID", filter.ManagerUserIdentifier));
            else if (parentUserId.HasValue)
                parameters.Add(new SqlParameter("ParentUserID", parentUserId));

            if (filter.DepartmentIdentifier.HasValue)
                parameters.Add(new SqlParameter("DepartmentIdentifier", filter.DepartmentIdentifier));

            if (filter.OrganizationIdentifier == Guid.Empty)
                throw new ArgumentOutOfRangeException("Invalid company: 0");

            parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            return parameters;
        }

        private static string CreateJoinForSelectSearchResults(EmployeeCompetencyFilter filter)
        {
            var join = new StringBuilder();

            join.Append($@"
OUTER APPLY (
    SELECT TOP 1
        TCollectionItem.ItemIdentifier AS CategoryIdentifier
       ,TCollectionItem.ItemName AS CategoryName
    FROM
        standards.StandardClassification WITH(NOLOCK) 
        INNER JOIN utilities.TCollectionItem WITH(NOLOCK) 
            ON TCollectionItem.ItemIdentifier = StandardClassification.CategoryIdentifier
    WHERE 
        StandardClassification.StandardIdentifier = c.StandardIdentifier
    ORDER BY
        StandardClassification.ClassificationSequence
) AS Classification
CROSS APPLY (
    select top 1
           DepartmentProfileCompetency.Criticality as PriorityName
         , DepartmentProfileCompetency.ValidForCount
         , DepartmentProfileCompetency.ValidForUnit
    from
        standards.DepartmentProfileUser as UserProfile WITH(NOLOCK) 
        inner join 
        custom_cmds.DepartmentProfileCompetency WITH(NOLOCK)  on DepartmentProfileCompetency.DepartmentIdentifier = UserProfile.DepartmentIdentifier
                                        and DepartmentProfileCompetency.ProfileStandardIdentifier = UserProfile.ProfileStandardIdentifier
        inner join
        custom_cmds.ProfileCompetency WITH(NOLOCK) on ProfileCompetency.ProfileStandardIdentifier = UserProfile.ProfileStandardIdentifier
                              and ProfileCompetency.CompetencyStandardIdentifier = DepartmentProfileCompetency.CompetencyStandardIdentifier
        inner join
        identities.Department WITH(NOLOCK) on Department.DepartmentIdentifier = UserProfile.DepartmentIdentifier
        {(filter.ManagerUserIdentifier.HasValue ? "INNER JOIN custom_cmds.Employment e ON e.UserIdentifier = ec.UserIdentifier AND e.DepartmentIdentifier = UserProfile.DepartmentIdentifier AND e.ProfileStandardIdentifier = UserProfile.ProfileStandardIdentifier" : string.Empty)}
    where
        UserProfile.UserIdentifier = ec.UserIdentifier
        and DepartmentProfileCompetency.CompetencyStandardIdentifier = ec.CompetencyStandardIdentifier
        and department.OrganizationIdentifier = @OrganizationIdentifier
        {(filter.ProfileStandardIdentifier.HasValue ? "AND UserProfile.ProfileStandardIdentifier = @ProfileStandardIdentifier" : string.Empty)}
        {(filter.IsComplianceRequired.HasValue ? (filter.IsComplianceRequired.Value ? "AND UserProfile.IsRequired = 1" : "AND UserProfile.IsRequired = 0") : string.Empty)}
        {(filter.DepartmentIdentifier.HasValue ? "AND Department.DepartmentIdentifier = @DepartmentIdentifier" : string.Empty)}
        {(filter.Criticality.IsNotEmpty() ? "AND DepartmentProfileCompetency.Criticality = @Criticality" : string.Empty)}
    order by
        PriorityName
    ) AS settings
");

            return join.ToString();
        }

        private static string CreateWhereForSelectSearchResults(EmployeeCompetencyFilter filter,
            Guid? validatorUserIdentifier, Guid? parentUserId)
        {
            var where = new StringBuilder("WHERE 1=1");

            if (filter.UserIdentifier == null && filter.UserDepartmentIdentifier == null)
                where.AppendFormat(@"
AND ec.UserIdentifier IN (
    SELECT DISTINCT
        m.UserIdentifier
    FROM
        contacts.Membership AS m
        INNER JOIN identities.Department ON Department.DepartmentIdentifier = m.GroupIdentifier
        WHERE
            Department.OrganizationIdentifier = @OrganizationIdentifier
)");

            where.AppendFormat(" AND c.IsDeleted = 0");

            if (filter.UserIdentifier.HasValue)
                where.Append(" AND ec.UserIdentifier = @UserIdentifier");
            else if (filter.UserDepartmentIdentifier.HasValue)
            {
                var employeeDepartmentWhere = string.IsNullOrEmpty(filter.EmployeeDepartmentAssignment)
                    ? null
                    : " AND MembershipType = @EmployeeDepartmentAssignment";

                where.AppendFormat(@"
AND ec.UserIdentifier IN (
    SELECT UserIdentifier
    FROM contacts.Membership
    WHERE DepartmentIdentifier = @EmployeeDepartmentIdentifier
        {0}
)", employeeDepartmentWhere);
            }

            if (filter.Statuses.IsNotEmpty())
            {
                var statusList = CsvConverter.ConvertListToCsvText(filter.Statuses, true);
                where.AppendFormat(" AND ec.ValidationStatus IN ({0})", statusList);
            }

            if (filter.ExcludeStatuses.IsNotEmpty())
            {
                var statusList = CsvConverter.ConvertListToCsvText(filter.ExcludeStatuses, true);
                where.AppendFormat(" AND ec.ValidationStatus NOT IN ({0})", statusList);
            }

            if (filter.IsValidated.HasValue)
            {
                if (filter.IsValidated.Value)
                    where.AppendFormat(" AND ec.IsValidated = 1");
                else
                    where.AppendFormat(" AND (ec.IsValidated = 0 OR ec.ValidationStatus = 'Expired')");
            }

            if (filter.CategoryIdentifier.HasValue)
                where.Append(" AND c.StandardIdentifier IN (SELECT StandardIdentifier FROM standards.StandardClassification WHERE CategoryIdentifier = @CategoryIdentifier)");

            if (filter.Keyword.IsNotEmpty())
                where.Append(" AND c.Summary LIKE @Keyword");

            if (filter.SelfAssessmentStatus.IsNotEmpty())
                where.Append(" AND ec.SelfAssessmentStatus = @SelfAssessmentStatus");

            if (filter.Number.IsNotEmpty())
                where.Append(" AND c.Number LIKE @Number");

            if (filter.NumberOld.IsNotEmpty())
                where.Append(" AND c.NumberOld LIKE @NumberOld");

            if (validatorUserIdentifier.HasValue)
            {
                // Work Orders 5463 and 5552: When the validator has a competency for which his validation 
                // status is Not Applicable, he is permitted to validate another person for the same 
                // competency with that same status. This is the reason I have added 'Not Applicable' to 
                // the WHERE clause.

                where.Append(@"
AND (
    EXISTS(
        SELECT *
            FROM identities.[User]
            WHERE UserIdentifier IN (SELECT UserIdentifier FROM custom_cmds.UserRole WHERE GroupName = 'CMDS Super Validators')
            AND UserIdentifier = @ValidatorUserIdentifier
        )
    OR ec.CompetencyStandardIdentifier IN (
            SELECT CompetencyStandardIdentifier
                FROM custom_cmds.UserCompetency x
                WHERE x.UserIdentifier = @ValidatorUserIdentifier
                AND ( (x.ValidationStatus IN ('Validated', 'Expired'))
                        OR
                        (x.ValidationStatus = 'Not Applicable' and ec.SelfAssessmentStatus = 'Not Applicable')
                    )
                AND ValidationDate IS NOT NULL
            )
    )

AND ec.UserIdentifier IN (
    SELECT ToUserIdentifier
        FROM identities.UserConnection
        WHERE FromUserIdentifier = @ValidatorUserIdentifier
        AND IsValidator = 1
)");
            }

            if (filter.ManagerUserIdentifier.HasValue)
            {
                where.Append(@"
AND ec.UserIdentifier IN (
    SELECT ToUserIdentifier
        FROM identities.UserConnection
        WHERE FromUserIdentifier = @ManagerID
        AND (IsManager = 1 OR IsSupervisor = 1)
)");
            }
            else if (parentUserId.HasValue)
            {
                where.Append(@"
AND (
        ec.UserIdentifier = @ParentUserID
    OR  ec.UserIdentifier IN (
            SELECT ToUserIdentifier
            FROM identities.UserConnection
            WHERE FromUserIdentifier = @ParentUserID
        )
    )");
            }

            if (filter.ValidationDateMustBeSet)
                where.Append(" AND ec.ValidationDate IS NOT NULL");

            if (filter.ValidationDateMustBeNull)
                where.Append(" AND ec.ValidationDate IS NULL");

            return where.ToString();
        }

        #endregion
    }
}
