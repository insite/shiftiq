using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class UserProfileRepository
    {
        #region UPDATE

        public static void ChangePrimaryUserProfile(Guid user, Guid profileStandardIdentifier, Guid department)
        {
            using (var db = new InternalDbContext())
            {
                var organizationId = db.Departments.FirstOrDefault(x => x.DepartmentIdentifier == department).OrganizationIdentifier;

                var affected = db.DepartmentProfileUsers
                    .Where(x =>
                        x.UserIdentifier == user
                        && x.Department.OrganizationIdentifier == organizationId
                        && (x.IsPrimary || x.DepartmentIdentifier == department)
                    )
                    .ToList();

                foreach (var entity in affected)
                    entity.IsPrimary = entity.ProfileStandardIdentifier == profileStandardIdentifier && entity.DepartmentIdentifier == department;

                db.SaveChanges();
            }
        }

        #endregion

        #region INSERT

        public static void RegisterNewProfile(
            bool isPrimary,
            Guid departmentIdentifier,
            Guid userIdentifier,
            Guid profileStandardIdentifier,
            bool isRecommended,
            bool isInProgress,
            bool isComplianceRequired
            )
        {
            using (var db = new InternalDbContext())
            {
                var userProfile = db.DepartmentProfileUsers.FirstOrDefault(x =>
                    x.UserIdentifier == userIdentifier
                    && x.DepartmentIdentifier == departmentIdentifier
                    && x.ProfileStandardIdentifier == profileStandardIdentifier
                    );

                if (userProfile != null)
                {
                    if (userProfile.IsRecommended != isRecommended || userProfile.IsInProgress != isInProgress || userProfile.IsPrimary != isPrimary)
                    {
                        userProfile.IsRecommended = isRecommended;
                        userProfile.IsInProgress = isInProgress;
                        userProfile.IsPrimary = isPrimary;
                        userProfile.IsRequired = isComplianceRequired;
                    }
                }
                else
                {
                    userProfile = new DepartmentProfileUser
                    {
                        DepartmentIdentifier = departmentIdentifier,
                        UserIdentifier = userIdentifier,
                        ProfileStandardIdentifier = profileStandardIdentifier,
                        IsRecommended = isRecommended,
                        IsInProgress = isInProgress,
                        IsPrimary = isPrimary,
                        IsRequired = isComplianceRequired
                    };

                    db.DepartmentProfileUsers.Add(userProfile);
                }

                db.SaveChanges();
            }

            UserCompetencyRepository.AddNewCompetencies(profileStandardIdentifier, new Guid[] { userIdentifier });
        }

        #endregion

        #region SELECT

        public static List<UserProfile> SelectByProfileStandardIdentifier(Guid profileStandardIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.UserProfiles.Where(x => x.ProfileStandardIdentifier == profileStandardIdentifier).ToList();
        }

        public static DataTable SelectEmployments(Guid userKey, Guid? organizationId, int? pageIndex, int? pageSize)
        {
            const string query = @"
SELECT
    organization.OrganizationIdentifier
   ,organization.CompanyTitle AS CompanyName
   ,departments.DepartmentIdentifier
   ,departments.DepartmentName
   ,profiles.ProfileStandardIdentifier
   ,profiles.ProfileTitle AS ProfileTitle
   ,profiles.ProfileNumber AS ProfileNumber
   ,e.UserIdentifier AS PersonID

  FROM custom_cmds.Employment e
  INNER JOIN accounts.QOrganization as organization ON organization.OrganizationIdentifier = e.OrganizationIdentifier
  LEFT JOIN identities.Department AS departments ON departments.DepartmentIdentifier = e.DepartmentIdentifier
  LEFT JOIN custom_cmds.[Profile] profiles ON profiles.ProfileStandardIdentifier = e.ProfileStandardIdentifier
  
  WHERE e.UserIdentifier = @UserIdentifier
    {0}
";
            string where = organizationId.HasValue ? "AND e.OrganizationIdentifier = @OrganizationIdentifier" : null;
            string curQuery = string.Format(query, where);

            if (pageIndex.HasValue)
            {
                curQuery += @"
ORDER BY CompanyName, DepartmentName, ProfileTitle
OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY
";
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("UserIdentifier", userKey)
            };

            if (organizationId.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", organizationId));

            if (pageIndex.HasValue)
            {
                parameters.Add(new SqlParameter("PageIndex", pageIndex));
                parameters.Add(new SqlParameter("PageSize", pageSize));
            }

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectGridWithCompetencyCount(Guid userKey, Guid organizationId, bool onlyPrimary, int? pageIndex, int? pageSize)
        {
            string query = @"
WITH Competencies AS
(
    SELECT
        DepartmentProfileUser.ProfileStandardIdentifier AS ProfileStandardIdentifier
       ,DepartmentProfileUser.UserIdentifier AS UserIdentifier
       ,DepartmentProfileUser.DepartmentIdentifier
       ,DepartmentProfileUser.IsPrimary
       ,DepartmentProfileUser.IsRequired AS IsComplianceRequired
       ,[Profile].Code AS ProfileNumber
       ,[Profile].ContentTitle AS ProfileTitle
       ,UserCompetency.ValidationStatus
       ,DepartmentProfileCompetency.Criticality
       ,Organization.CompanyTitle AS CompanyName
       ,Department.DepartmentName
       ,CASE 
	    WHEN DepartmentProfileUser.IsRecommended = 1 THEN 'Required for Promotion'
	    WHEN DepartmentProfileUser.IsInProgress = 1 THEN 'In Training'
	    ELSE NULL
	    END AS StatusText
    FROM
        standards.DepartmentProfileUser
        INNER JOIN standards.[Standard] AS [Profile] ON [Profile].StandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
        INNER JOIN identities.Department ON Department.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier
        INNER JOIN accounts.QOrganization AS Organization ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
        INNER JOIN standards.StandardContainment AS ProfileCompetency ON ProfileCompetency.ParentStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
        INNER JOIN standards.StandardValidation AS UserCompetency ON UserCompetency.StandardIdentifier = ProfileCompetency.ChildStandardIdentifier
                                                    AND UserCompetency.UserIdentifier = DepartmentProfileUser.UserIdentifier
        INNER JOIN custom_cmds.DepartmentProfileCompetency ON DepartmentProfileCompetency.ProfileStandardIdentifier = DepartmentProfileUser.ProfileStandardIdentifier
                                                                        AND DepartmentProfileCompetency.CompetencyStandardIdentifier = ProfileCompetency.ChildStandardIdentifier
                                                                        AND DepartmentProfileCompetency.DepartmentIdentifier = DepartmentProfileUser.DepartmentIdentifier
    WHERE
        DepartmentProfileUser.UserIdentifier = @UserIdentifier
        AND Department.OrganizationIdentifier = @OrganizationIdentifier
        AND (@OnlyPrimary = 0 OR DepartmentProfileUser.IsPrimary = 1)
)
SELECT
    ProfileStandardIdentifier
   ,UserIdentifier
   ,DepartmentIdentifier
   ,IsPrimary
   ,IsComplianceRequired
   ,ProfileNumber
   ,ProfileTitle
   ,SUM(CASE WHEN (
                    Criticality IS NULL
                    OR Criticality <> 'Critical'
                    ) THEN 1
                ELSE 0
        END) NonCriticalCompetencies
   ,SUM(CASE WHEN Criticality = 'Critical' THEN 1
                ELSE 0
        END) CriticalCompetencies
   ,SUM(CASE WHEN (
                    Criticality IS NULL
                    OR Criticality <> 'Critical'
                    )
                    AND ( ValidationStatus in ( 'Self-Assessed' ) ) THEN 1
                ELSE 0
        END) NonCriticalCompetencies_SelfAssessed
   ,SUM(CASE WHEN Criticality = 'Critical'
                    AND ( ValidationStatus in ( 'Self-Assessed' ) ) THEN 1
                ELSE 0
        END) CriticalCompetencies_SelfAssessed
   ,SUM(CASE WHEN (
                    Criticality IS NULL
                    OR Criticality <> 'Critical'
                    )
                    AND ( ValidationStatus in ( 'Submitted for Validation' ) ) THEN 1
                ELSE 0
        END) NonCriticalCompetencies_Submitted
   ,SUM(CASE WHEN Criticality = 'Critical'
                    AND ( ValidationStatus in ( 'Submitted for Validation' ) ) THEN 1
                ELSE 0
        END) CriticalCompetencies_Submitted
   ,SUM(CASE WHEN (
                    Criticality IS NULL
                    OR Criticality <> 'Critical'
                    )
                    AND ( ValidationStatus = 'Validated' ) THEN 1
                ELSE 0
        END) NonCriticalCompetencies_Validated
   ,SUM(CASE WHEN Criticality = 'Critical'
                    AND ( ValidationStatus = 'Validated' ) THEN 1
                ELSE 0
        END) CriticalCompetencies_Validated
    ,CompanyName
    ,DepartmentName
    ,StatusText
FROM
    Competencies
GROUP BY
    ProfileStandardIdentifier
   ,UserIdentifier
   ,DepartmentIdentifier
   ,IsPrimary
   ,IsComplianceRequired
   ,ProfileNumber
   ,ProfileTitle
   ,CompanyName
   ,DepartmentName
   ,StatusText
            ";

            if (pageIndex.HasValue)
            {
                query += @"
ORDER BY IsPrimary DESC, DepartmentName, ProfileNumber
OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY
";
            }

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("OnlyPrimary", onlyPrimary)
            };

            if (pageIndex.HasValue)
            {
                parameters.Add(new SqlParameter("PageIndex", pageIndex));
                parameters.Add(new SqlParameter("PageSize", pageSize));
            }

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static DataTable SelectForSelector(Guid userKey, Guid organizationId, bool includePrimary)
        {
            const string query = @"
                SELECT
                    (
                        CAST(ep.UserIdentifier AS NVARCHAR(50)) + ';' +
                        CAST(ep.ProfileStandardIdentifier AS NVARCHAR(50)) + ';' +
                        CAST(ep.DepartmentIdentifier AS NVARCHAR(50))
                    ) AS Value,
                    (
                        ISNULL(p.ProfileNumber, '') + ': ' +
                        p.ProfileTitle + ' (' +
                        department.DepartmentName + ', ' +
                        (CASE WHEN ep.CurrentStatus IS NOT NULL THEN ep.CurrentStatus + ', ' ELSE '' END)
                        + ')'
                    ) AS Text,
                    ep.IsPrimary
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.[Profile] p ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND Department.OrganizationIdentifier = @OrganizationIdentifier
                    AND ep.IsPrimary <= @IsPrimary
                  ORDER BY ep.IsPrimary DESC, p.ProfileNumber, Department.DepartmentName
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("IsPrimary", includePrimary)
                );
        }

        public static DataTable SelectEmployees(Guid departmentKey, Guid profileStandardIdentifier)
        {
            const string query = @"
SELECT [User].*
      ,[User].Email AS EmailWork
      ,[User].UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
  FROM custom_cmds.UserProfile
       INNER JOIN identities.[User] ON [User].UserIdentifier = UserProfile.UserIdentifier
  WHERE UserProfile.DepartmentIdentifier = @DepartmentIdentifier
    AND UserProfile.ProfileStandardIdentifier = @ProfileStandardIdentifier
  ORDER BY FullName";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("DepartmentIdentifier", departmentKey),
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier)
                );
        }

        public static bool HasOtherProfiles(Guid organizationId, Guid userKey)
        {
            const string query = @"
                SELECT TOP 1 1
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN identities.Department
                    ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND Department.OrganizationIdentifier <> @OrganizationIdentifier
                ";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int?>(query, new SqlParameter("OrganizationIdentifier", organizationId), new SqlParameter("UserIdentifier", userKey)).FirstOrDefault().HasValue;
        }

        public static DataTable SelectOtherProfiles(Guid organizationId, Guid userKey)
        {
            const string query = @"
                SELECT DISTINCT
                    Organization.CompanyTitle AS CompanyName,
                    department.DepartmentName,
                    p.ProfileNumber AS ProfileNumber,
                    p.ProfileTitle AS ProfileTitle
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.[Profile] p
                    ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department
                    ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
                  INNER JOIN accounts.QOrganization AS Organization
                    ON Organization.OrganizationIdentifier = department.OrganizationIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND department.OrganizationIdentifier <> @OrganizationIdentifier
                  ORDER BY
                    CompanyName,
                    DepartmentName,
                    ProfileNumber
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("UserIdentifier", userKey)
                );
        }

        public static DataTable SelectSecondaryProfilesInTraining(Guid userKey, Guid organizationId)
        {
            const string query = @"
                SELECT
                    p.ProfileStandardIdentifier,
                    p.ProfileNumber AS ProfileNumber,
                    p.ProfileTitle AS ProfileTitle,
                    p.OrganizationIdentifier,
                    department.DepartmentIdentifier,
                    department.DepartmentName
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.[Profile] p
                    ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department
                    ON department.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND ep.CurrentStatus = 'In Training'
                    AND ep.ProfileStandardIdentifier NOT IN (
                          SELECT ProfileStandardIdentifier
                            FROM custom_cmds.Employment
                            WHERE UserIdentifier = @UserIdentifier
                              AND OrganizationIdentifier = @OrganizationIdentifier
                        )
                    AND department.OrganizationIdentifier = @OrganizationIdentifier
                  ORDER BY p.ProfileNumber, p.ProfileTitle
                ";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("UserIdentifier", userKey)
                );
        }

        public static DataTable SelectEmployeeProfilesByCompany(Guid userKey, Guid organizationId)
        {
            const string query = @"
                SELECT
                    ep.*,
                    p.ProfileTitle AS ProfileTitle,
                    department.DepartmentName
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.[Profile] p
                    ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department
                    ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND department.OrganizationIdentifier = @OrganizationIdentifier
                  ORDER BY IsPrimary DESC, DepartmentName, p.ProfileNumber
";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("UserIdentifier", userKey)
                );
        }

        public static UserProfile SelectPrimaryProfile(Guid userKey, Guid organizationId)
        {
            const string query = @"
SELECT UserProfile.*
FROM custom_cmds.UserProfile 
INNER JOIN custom_cmds.Employment ON UserProfile.ProfileStandardIdentifier = Employment.ProfileStandardIdentifier
AND UserProfile.DepartmentIdentifier = Employment.DepartmentIdentifier
AND Employment.OrganizationIdentifier = @OrganizationIdentifier
WHERE UserProfile.UserIdentifier = @UserIdentifier
AND UserProfile.IsPrimary = 1
                ";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<UserProfile>(query,
                    new SqlParameter("UserIdentifier", userKey),
                    new SqlParameter("OrganizationIdentifier", organizationId)
                    ).FirstOrDefault();
            }
        }

        public static DataTable SelectSecondaryProfiles(Guid userKey, Guid organizationId, bool? isComplianceRequired)
        {
            return SelectSecondaryProfiles(userKey, organizationId, isComplianceRequired, false);
        }

        private static DataTable SelectSecondaryProfiles(Guid userKey, Guid organizationId, bool? isComplianceRequired, bool getCount)
        {
            const string query = @"
                SELECT {1}
                  FROM custom_cmds.UserProfile ep
                  INNER JOIN custom_cmds.[Profile] p ON p.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
                  INNER JOIN identities.Department ON department.DepartmentIdentifier = ep.DepartmentIdentifier
                  WHERE ep.UserIdentifier = @UserIdentifier
                    AND department.OrganizationIdentifier = @OrganizationIdentifier
                    AND ep.ProfileStandardIdentifier NOT IN (
                          SELECT ProfileStandardIdentifier
                            FROM custom_cmds.Employment
                            WHERE UserIdentifier = @UserIdentifier
                              AND OrganizationIdentifier = @OrganizationIdentifier
                        )
                    {0}
                  {2}
                ";

            string where = isComplianceRequired.HasValue ? " AND ep.IsComplianceRequired = @IsComplianceRequired" : null;
            string curQuery = string.Format(
                query
              , where
              , getCount
                    ? "COUNT(*) AS RowsCount"
                    : "DISTINCT p.ProfileStandardIdentifier, p.ProfileTitle AS ProfileName, p.ProfileNumber AS ProfileNumber, ep.UserIdentifier, ep.DepartmentIdentifier, ep.CurrentStatus AS ProfileStatusName"
              , getCount
                    ? string.Empty
                    : "ORDER BY p.ProfileNumber, p.ProfileTitle"
            );

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("UserIdentifier", userKey),
                new SqlParameter("OrganizationIdentifier", organizationId)
            };

            if (isComplianceRequired.HasValue)
                parameters.Add(new SqlParameter("IsComplianceRequired", isComplianceRequired));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        #endregion
    }
}
