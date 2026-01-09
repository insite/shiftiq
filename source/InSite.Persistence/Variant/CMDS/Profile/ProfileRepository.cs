using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Constant;
using Shift.Constant.CMDS;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ProfileRepository
    {
        #region SELECT

        public static Profile Select(Guid profileStandardIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.Profiles.FirstOrDefault(x => x.ProfileStandardIdentifier == profileStandardIdentifier);
            }
        }

        public static Profile Select(string number)
        {
            using (var db = new InternalDbContext())
            {
                return db.Profiles.FirstOrDefault(x => x.ProfileNumber == number);
            }
        }

        public static DataTable SelectProfilesForDepartment(Guid department, Guid competencyStandardIdentifier)
        {
            const string query = @"
SELECT
    dp.ProfileStandardIdentifier AS ProfileStandardIdentifier,
    (
      CASE
        WHEN cs.CompetencyStandardIdentifier IS NOT NULL THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
      END
    ) AS HasCompetency
  FROM custom_cmds.DepartmentProfile dp
  INNER JOIN custom_cmds.ProfileCompetency pc
    ON pc.ProfileStandardIdentifier = dp.ProfileStandardIdentifier
  LEFT JOIN custom_cmds.DepartmentProfileCompetency cs
    ON cs.DepartmentIdentifier = dp.DepartmentIdentifier
      AND cs.ProfileStandardIdentifier = dp.ProfileStandardIdentifier
      AND cs.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
  WHERE dp.DepartmentIdentifier = @DepartmentIdentifier
    AND pc.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                ";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("DepartmentIdentifier", department), new SqlParameter("CompetencyStandardIdentifier", competencyStandardIdentifier));
        }

        public static DataTable SelectProfilesForPerson(Guid department, Guid userKey)
        {
            const string query = @"
SELECT
    p.ProfileStandardIdentifier,
    p.ProfileTitle,
    ep.IsPrimary,
    ep.IsComplianceRequired
FROM
    custom_cmds.[Profile] p
    INNER JOIN custom_cmds.DepartmentProfile dp
        ON dp.ProfileStandardIdentifier = p.ProfileStandardIdentifier
    LEFT JOIN custom_cmds.UserProfile ep 
        ON ep.ProfileStandardIdentifier = dp.ProfileStandardIdentifier
           AND ep.DepartmentIdentifier = dp.DepartmentIdentifier
           AND ep.UserIdentifier = @UserIdentifier
WHERE
    dp.DepartmentIdentifier = @DepartmentIdentifier
ORDER BY
    p.ProfileTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("DepartmentIdentifier", department), new SqlParameter("UserIdentifier", userKey));
        }

        #endregion

        #region SELECT (legacy)

        public static DataTable SelectRelatedGroups(Guid profileStandardIdentifier)
        {
            const string query = @"
SELECT
    Organization.OrganizationIdentifier
   ,Organization.CompanyTitle AS CompanyName
   ,NULL AS DepartmentIdentifier
   ,NULL AS DepartmentName
FROM
    custom_cmds.VCmdsProfileOrganization AS cp
    INNER JOIN accounts.QOrganization AS Organization
        ON Organization.OrganizationIdentifier = cp.OrganizationIdentifier
WHERE
    cp.ProfileStandardIdentifier = @ProfileStandardIdentifier

UNION ALL

SELECT
    Organization.OrganizationIdentifier
   ,Organization.CompanyTitle AS CompanyName
   ,Department.DepartmentIdentifier
   ,Department.DepartmentName
FROM
    custom_cmds.DepartmentProfile AS dp
    INNER JOIN identities.Department
        ON Department.DepartmentIdentifier = dp.DepartmentIdentifier
    INNER JOIN accounts.QOrganization AS Organization
        ON Organization.OrganizationIdentifier = Department.OrganizationIdentifier
WHERE
    dp.ProfileStandardIdentifier = @ProfileStandardIdentifier

ORDER BY
    CompanyName
   ,DepartmentName;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier));
        }

        public static DataTable SelectRelatedEmployees(Guid profileStandardIdentifier)
        {
            const string query = @"
SELECT DISTINCT
    p.UserIdentifier,
    p.FullName,
    organization.OrganizationIdentifier,
    organization.CompanyTitle AS CompanyName
FROM
    custom_cmds.UserProfile ep
    INNER JOIN identities.[User] p
        ON p.UserIdentifier = ep.UserIdentifier
    INNER JOIN identities.Department
        ON Department.DepartmentIdentifier = ep.DepartmentIdentifier
    INNER JOIN accounts.QOrganization as organization
        ON organization.OrganizationIdentifier = Department.OrganizationIdentifier
WHERE
    ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
ORDER BY
    p.FullName, organization.CompanyTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier));
        }

        public static string SelectMaxNumber()
        {
            const string query = @"
SELECT MAX(ProfileNumber)
FROM custom_cmds.[Profile]
WHERE ProfileNumber LIKE '[0-9][0-9][0-9][0-9]';";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query).FirstOrDefault();
        }

        public static string SelectMaxNumber(string acronym)
        {
            const string query = @"
SELECT MAX(ProfileNumber)
FROM custom_cmds.[Profile]
WHERE ProfileNumber LIKE @ProfileNumber;";

            var number = string.Format("{0}-[0-9][0-9][0-9][0-9]", acronym);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query, new SqlParameter("ProfileNumber", number)).FirstOrDefault();
        }

        public static DataTable SelectNewDepartmentProfiles(Guid departmentKey, string searchText)
        {
            var query = new StringBuilder(@"
SELECT
    p.*
FROM
    custom_cmds.[Profile] p
    INNER JOIN custom_cmds.VCmdsProfileOrganization cp
        ON cp.ProfileStandardIdentifier = p.ProfileStandardIdentifier
    INNER JOIN identities.Department as dep
        ON dep.OrganizationIdentifier = cp.OrganizationIdentifier
WHERE
    dep.DepartmentIdentifier = @DepartmentIdentifier
    AND (
            p.Visibility IS NULL
        OR p.Visibility = @AccountScope
        OR p.OrganizationIdentifier = cp.OrganizationIdentifier
        )
    AND p.ProfileStandardIdentifier NOT IN (
        SELECT ProfileStandardIdentifier
            FROM custom_cmds.DepartmentProfile
            WHERE DepartmentIdentifier = dep.DepartmentIdentifier
        )");

            if (!string.IsNullOrEmpty(searchText))
                query.Append(" AND (p.ProfileTitle LIKE @SearchText OR p.ProfileNumber LIKE @SearchText)");

            query.Append("ORDER BY p.ProfileNumber, p.ProfileTitle");

            return DatabaseHelper.CreateDataTable(
                query.ToString(),
                new SqlParameter("DepartmentIdentifier", departmentKey),
                new SqlParameter("SearchText", string.Format("%{0}%", searchText)),
                new SqlParameter("AccountScope", AccountScopes.Enterprise)
                );
        }

        public static DataTable SelectCompanyProfilesForDepartment(Guid organizationId, Guid department)
        {
            const string query = @"
SELECT
    p.ProfileStandardIdentifier,
    p.ProfileNumber,
    p.ProfileTitle,
    CASE
        WHEN dp.ProfileStandardIdentifier IS NOT NULL THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END AS IsSelected
FROM
    custom_cmds.[Profile] p
    INNER JOIN custom_cmds.VCmdsProfileOrganization cp
        ON cp.ProfileStandardIdentifier = p.ProfileStandardIdentifier
    LEFT JOIN custom_cmds.DepartmentProfile dp
        ON dp.ProfileStandardIdentifier = cp.ProfileStandardIdentifier
           AND dp.DepartmentIdentifier = @DepartmentIdentifier
WHERE
    cp.OrganizationIdentifier = @OrganizationIdentifier
ORDER BY
    p.ProfileNumber, p.ProfileTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("DepartmentIdentifier", department), new SqlParameter("OrganizationIdentifier", organizationId));
        }

        public static DataTable SelectDepartmentProfilesOnly(Guid department)
        {
            const string query = @"
SELECT
    p.*
FROM
    custom_cmds.[Profile] AS p
    INNER JOIN custom_cmds.DepartmentProfile AS dp
        ON dp.ProfileStandardIdentifier = p.ProfileStandardIdentifier
WHERE
    dp.DepartmentIdentifier = @DepartmentIdentifier
ORDER BY
    p.ProfileNumber
   ,p.ProfileTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("DepartmentIdentifier", department));
        }

        public static DataTable SelectNewOrganizationProfiles(Guid organizationId, string searchText)
        {
            var query = new StringBuilder(@"
SELECT
    *
FROM
    custom_cmds.[Profile]
WHERE
    ProfileStandardIdentifier NOT IN (
        SELECT ProfileStandardIdentifier
        FROM custom_cmds.VCmdsProfileOrganization
        WHERE OrganizationIdentifier = @OrganizationIdentifier
    )
    AND (ParentProfileStandardIdentifier IS NULL OR OrganizationIdentifier = @OrganizationIdentifier)");

            if (!string.IsNullOrEmpty(searchText))
                query.Append(" AND (ProfileTitle LIKE @SearchText OR ProfileNumber LIKE @SearchText)");

            query.Append("ORDER BY ProfileNumber, ProfileTitle");

            return DatabaseHelper.CreateDataTable(query.ToString(), new SqlParameter("OrganizationIdentifier", organizationId), new SqlParameter("SearchText", string.Format("%{0}%", searchText)));
        }

        public static DataTable SelectOrganizationProfilesOnly(Guid organization)
        {
            const string query = @"
SELECT
    p.*
FROM
    custom_cmds.[Profile] p
    INNER JOIN custom_cmds.VCmdsProfileOrganization cp
        ON cp.ProfileStandardIdentifier = p.ProfileStandardIdentifier
WHERE
    cp.OrganizationIdentifier = @OrganizationIdentifier
ORDER BY
    p.ProfileNumber, p.ProfileTitle;";

            return DatabaseHelper.CreateDataTable(query.ToString(), new SqlParameter("OrganizationIdentifier", organization));
        }

        public static List<Profile> SelectEmployeeProfiles(Guid employeeId, Guid competencyStandardIdentifier, Guid organizationId)
        {
            const string query = @"
SELECT DISTINCT
    p.*
FROM
    custom_cmds.[Profile] p
    INNER JOIN custom_cmds.UserProfile ep
        ON ep.ProfileStandardIdentifier = p.ProfileStandardIdentifier
    INNER JOIN custom_cmds.ProfileCompetency pc
        ON pc.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
    INNER JOIN custom_cmds.UserCompetency ec
        ON ec.UserIdentifier = ep.UserIdentifier
           AND ec.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
    INNER JOIN custom_cmds.VCmdsProfileOrganization cp
        ON cp.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
WHERE
    ec.UserIdentifier = @UserIdentifier
    AND ec.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
    AND cp.OrganizationIdentifier = @OrganizationIdentifier
ORDER BY
    p.ProfileTitle;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Profile>(query, new SqlParameter("UserIdentifier", employeeId), new SqlParameter("CompetencyStandardIdentifier", competencyStandardIdentifier), new SqlParameter("OrganizationIdentifier", organizationId)).ToList();
        }

        #endregion

        #region Select for selector

        public static bool SelectTextForSelector(Guid profileStandardIdentifier, out string text)
        {
            text = null;

            var info = StandardSearch.Select(profileStandardIdentifier);
            if (info == null)
                return false;

            string prefix;

            if (info.OrganizationIdentifier != OrganizationIdentifiers.CMDS)
            {
                var organization = OrganizationSearch.Select(info.OrganizationIdentifier);

                prefix = organization.CompanyName + " ";
            }
            else
                prefix = null;

            text = $"{prefix}{info.ContentTitle} ({info.Code})";

            return true;
        }

        private const string QueryBodyForSelector = @"
SELECT
    p.ProfileStandardIdentifier,
    p.OrganizationIdentifier,
    p.ParentProfileStandardIdentifier,
    null AS Category,
    p.ProfileNumber,
    p.ProfileTitle,
    p.ProfileDescription,
    p.CertificationIsAvailable,
    p.ProfileStandardIdentifier AS [Value],
    p.Visibility,

    CASE
        WHEN p.Visibility = 'Organization' THEN ISNULL(Organization.CompanyName, '') + ' ' + p.ProfileTitle + ' (' + ISNULL(p.ProfileNumber, '') + ')'
        ELSE p.ProfileTitle + ' (' + ISNULL(p.ProfileNumber, '') + ')'
    END AS [Text],
    CASE
        WHEN p.Visibility = 'Organization' THEN Organization.CompanyName
        ELSE NULL
    END AS Initials,
    CASE
        WHEN p.Visibility = 'Organization' THEN 1
        ELSE 0
    END AS SortOrder
FROM
    custom_cmds.[Profile] p
    INNER JOIN accounts.QOrganization AS Organization ON Organization.OrganizationIdentifier = p.OrganizationIdentifier";
        private const string QueryForSelector = @"
WITH OrderedProfiles AS
(
    SELECT *, ROW_NUMBER() OVER(ORDER BY SortOrder, Initials, Text) AS RowNumber
    FROM (" + QueryBodyForSelector + @") t
    {0}
)
SELECT * FROM OrderedProfiles
{1}
ORDER BY SortOrder, Initials, [Text]

";

        public static DataTable SelectForSelector(ProfileFilter filter, string searchText)
        {
            var where = CreateWhereForFilter(filter, searchText);
            var query = string.Format(QueryForSelector, where, "WHERE RowNumber BETWEEN @StartRow AND @EndRow");

            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForFilter(filter, searchText);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static DataTable SelectForSelector(IEnumerable<Guid> ids)
        {
            var where = $"WHERE [Value] IN ({CsvConverter.ConvertListToCsvText(ids, true)})";
            var query = string.Format(QueryForSelector, where, null);

            return DatabaseHelper.CreateDataTable(query);
        }

        public static int CountForSelector(ProfileFilter filter, string searchText)
        {
            string where = CreateWhereForFilter(filter, searchText);
            string query = $"SELECT COUNT(*) FROM (" + QueryBodyForSelector + ") t " + where;

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForFilter(filter, searchText).ToArray()).FirstOrDefault();
        }

        #endregion

        #region SelectSearchResults

        public static DataTable SelectSearchResults(ProfileFilter filter)
        {
            var where = CreateWhereForFilter(filter, null);

            var sortExpression = "ProfileNumber, ProfileTitle";

            var competencyCountField = @"
                (
                    SELECT COUNT(*)
                        FROM custom_cmds.ProfileCompetency pc
                        INNER JOIN custom_cmds.Competency c
                        ON c.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
                        WHERE pc.ProfileStandardIdentifier = p.ProfileStandardIdentifier
                        AND c.IsDeleted = 0
                )";

            var acquiredCountField = @"
                (
                    SELECT COUNT(*)
                      FROM custom_cmds.UserProfile
                      WHERE ProfileStandardIdentifier = p.ProfileStandardIdentifier
                )";

            var withSortExpression = sortExpression
                .Replace("CompetencyCount", competencyCountField)
                .Replace("AcquiredCount", acquiredCountField)
                ;

            var query = string.Format(@"
                WITH OrderedProfiles AS
                (
                  SELECT
                        *,
                        {3} AS CompetencyCount,
                        {4} AS AcquiredCount,
                        ROW_NUMBER() OVER(ORDER BY {2}) AS RowNumber
                    FROM custom_cmds.[Profile] p
                    {0}
                )
                SELECT * FROM OrderedProfiles
                WHERE RowNumber BETWEEN @StartRow AND @EndRow
                ORDER BY {1}
                ", where, sortExpression, withSortExpression, competencyCountField, acquiredCountField);

            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForFilter(filter, null);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static int CountSearchResults(ProfileFilter filter)
        {
            string where = CreateWhereForFilter(filter, null);

            string query = string.Format(@"
                  SELECT COUNT(*)
                    FROM custom_cmds.[Profile]
                    {0}
                ", where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForFilter(filter, null).ToArray()).FirstOrDefault();
        }

        #endregion

        #region Create where for filter

        private static string CreateWhereForFilter(ProfileFilter filter, string searchText)
        {
            StringBuilder where = new StringBuilder();
            where.Append("WHERE 1 = 1");

            if (filter.IsHierarchySelect)
            {
                if (filter.ProfileOrganizationIdentifier.HasValue)
                {
                    where.Append(@" AND OrganizationIdentifier = @ProfileOrganizationIdentifier");
                }
                else
                {
                    if (!filter.AddProfilesFromOrganizationIdentifier.HasValue)
                        where.Append(" AND ParentProfileStandardIdentifier IS NULL");
                }
            }
            else
            {
                if (filter.ProfileOrganizationIdentifier.HasValue)
                    where.Append(" AND OrganizationIdentifier = @ProfileOrganizationIdentifier");

                if (!string.IsNullOrEmpty(filter.ProfileVisibility))
                    where.Append(" AND Visibility = @ProfileVisibility");

                if (filter.ParentProfileStandardIdentifier.HasValue)
                    where.Append(" AND ParentProfileStandardIdentifier = @ParentProfileStandardIdentifier");
            }

            if (filter.AddProfilesFromOrganizationIdentifier.HasValue)
                where.Append(@"
                            AND (ParentProfileStandardIdentifier IS NULL 
                             OR (ParentProfileStandardIdentifier IS NOT NULL AND ProfileStandardIdentifier IN (
                                SELECT ProfileStandardIdentifier
                                  FROM custom_cmds.VCmdsProfileOrganization
                                WHERE OrganizationIdentifier = @AddProfilesFromOrganizationIdentifier
                                ))
                            )");

            if (filter.DepartmentIdentifier.HasValue)
            {
                where.Append(@"
                    AND ProfileStandardIdentifier IN (
                        SELECT ProfileStandardIdentifier
                            FROM custom_cmds.DepartmentProfile
                            WHERE DepartmentIdentifier = @DepartmentIdentifier
                        )
                    ");
            }
            else if (filter.OrganizationIdentifier.HasValue)
            {
                where.Append(@"
                    AND ProfileStandardIdentifier IN (
                        SELECT ProfileStandardIdentifier
                            FROM custom_cmds.VCmdsProfileOrganization
                            WHERE OrganizationIdentifier = @OrganizationIdentifier
                        )
                    ");
            }
            else if (filter.CompaniesForPersonId.HasValue)
            {
                where.Append(@"
                    AND ProfileStandardIdentifier IN (
                          SELECT cp.ProfileStandardIdentifier
                            FROM custom_cmds.VCmdsProfileOrganization cp
                            INNER JOIN identities.Department as departments
                              ON departments.OrganizationIdentifier = cp.OrganizationIdentifier
                            INNER JOIN contacts.Membership as m
                              ON m.GroupIdentifier = departments.DepartmentIdentifier
                            WHERE m.UserIdentifier = @CompaniesForPersonID
                        )
                    ");
            }

            if (filter.ProfileUserIdentifier.HasValue)
            {
                where.Append(@"
                    AND ProfileStandardIdentifier IN (
                        SELECT ProfileStandardIdentifier
                          FROM custom_cmds.UserProfile
                          WHERE UserIdentifier = @ProfileUserIdentifier
                        )
                    ");
            }

            if (!string.IsNullOrEmpty(filter.Category))
            {
                where.Append(" AND Category LIKE @Category");

                if (filter.Category == "Certificate")
                    where.Append(" AND CertificationIsAvailable = 1");
            }

            if (!string.IsNullOrEmpty(filter.ProfileNumber))
                where.Append(" AND ProfileNumber LIKE @ProfileNumber");

            if (!string.IsNullOrEmpty(filter.ProfileTitle))
                where.Append(" AND ProfileTitle LIKE @ProfileTitle");

            if (!string.IsNullOrEmpty(filter.ProfileDescription))
                where.Append(" AND ProfileDescription LIKE @Description");

            if (filter.ExcludeProfileStandardIdentifier.HasValue)
                where.Append(" AND ProfileStandardIdentifier <> @ExcludeProfileStandardIdentifier");

            if (!string.IsNullOrEmpty(searchText))
                where.Append(" AND [Text] LIKE @SearchText");

            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForFilter(ProfileFilter filter, string searchText)
        {
            var parameters = new List<SqlParameter>();

            if (filter.ProfileOrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileOrganizationIdentifier", filter.ProfileOrganizationIdentifier));

            if (!string.IsNullOrEmpty(filter.ProfileVisibility))
                parameters.Add(new SqlParameter("ProfileVisibility", filter.ProfileVisibility));

            if (filter.ParentProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ParentProfileStandardIdentifier", filter.ParentProfileStandardIdentifier));

            if (filter.AddProfilesFromOrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("AddProfilesFromOrganizationIdentifier", filter.AddProfilesFromOrganizationIdentifier));

            if (filter.OrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            if (filter.DepartmentIdentifier.HasValue)
                parameters.Add(new SqlParameter("DepartmentIdentifier", filter.DepartmentIdentifier));

            if (filter.CompaniesForPersonId.HasValue)
                parameters.Add(new SqlParameter("CompaniesForPersonID", filter.CompaniesForPersonId));

            if (filter.ExcludeUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ExcludeUserIdentifier", filter.ExcludeUserIdentifier));

            if (filter.ProfileUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileUserIdentifier", filter.ProfileUserIdentifier));

            if (!string.IsNullOrEmpty(filter.Category))
                parameters.Add(new SqlParameter("Category", filter.Category));

            if (!string.IsNullOrEmpty(filter.ProfileNumber))
                parameters.Add(new SqlParameter("ProfileNumber", string.Format("%{0}%", filter.ProfileNumber)));

            if (!string.IsNullOrEmpty(filter.ProfileTitle))
                parameters.Add(new SqlParameter("ProfileTitle", string.Format("%{0}%", filter.ProfileTitle)));

            if (!string.IsNullOrEmpty(filter.ProfileDescription))
                parameters.Add(new SqlParameter("Description", string.Format("%{0}%", filter.ProfileDescription)));

            if (filter.ExcludeProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ExcludeProfileStandardIdentifier", filter.ExcludeProfileStandardIdentifier));

            if (!string.IsNullOrEmpty(searchText))
                parameters.Add(new SqlParameter("SearchText", string.Format("%{0}%", searchText)));

            return parameters;
        }

        #endregion
    }
}
