using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ContactRepository3
    {
        #region SELECT

        public static List<String> SelectUserRoles(Guid userId)
        {
            const string query = @"
select g.GroupName as RoleName
from contacts.Membership        as m
     inner join contacts.QGroup as g on g.GroupIdentifier = m.GroupIdentifier
where m.UserIdentifier = @UserIdentifier and g.GroupType = 'Role'
order by g.GroupName
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<string>(query, new SqlParameter("UserIdentifier", userId)).ToList();
        }

        public static DataTable SelectEmployeesByOrganizationIdentifier(Guid organizationId)
        {
            const string query = @"
SELECT p.UserIdentifier
      ,p.FullName + (CASE WHEN Person.EmployeeType IS NOT NULL THEN ' [' + Person.EmployeeType + ']' ELSE '' END) AS FullName
      ,p.Email
FROM identities.QUser AS p
INNER JOIN contacts.QPerson AS Person on p.UserIdentifier = Person.UserIdentifier and Person.OrganizationIdentifier = @OrganizationIdentifier
WHERE p.AccessGrantedToCmds = 1 AND EXISTS
(
    SELECT *
    FROM contacts.Membership AS m
        INNER JOIN identities.Department
            ON department.DepartmentIdentifier = m.GroupIdentifier
    WHERE m.UserIdentifier = p.UserIdentifier
          AND department.OrganizationIdentifier = @OrganizationIdentifier
          AND m.MembershipType = 'Department'
          AND p.UtcArchived IS NULL
)
ORDER BY FullName;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("OrganizationIdentifier", organizationId));
        }

        public static DataTable SelectEmployeesByDepartmentId(Guid department)
        {
            const string query = @"
SELECT p.UserIdentifier
      ,p.FullName + (CASE WHEN Person.EmployeeType IS NOT NULL THEN ' [' + Person.EmployeeType + ']' ELSE '' END) AS FullName
      ,p.Email
  FROM identities.QUser AS p 
  INNER JOIN contacts.Membership AS m ON m.UserIdentifier = p.UserIdentifier
  INNER JOIN contacts.QGroup AS Department ON m.GroupIdentifier = Department.GroupIdentifier
  INNER JOIN contacts.QPerson AS Person on p.UserIdentifier = Person.UserIdentifier and Person.OrganizationIdentifier = Department.OrganizationIdentifier
  WHERE m.GroupIdentifier = @DepartmentIdentifier
    AND m.MembershipType = 'Department'
    AND p.UtcArchived IS NULL
  ORDER BY p.FullName;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("DepartmentIdentifier", department));
        }

        public static DataTable SelectEmployeesByDepartmentProfileId(Guid department, Guid profileStandardIdentifier, bool isPrimaryProfile)
        {
            const string query = @"
SELECT
           p.*
         , p.Email AS EmailWork
         , CASE
               WHEN ep.UserIdentifier IS NULL
                   THEN
                   CAST(0 AS BIT)
               WHEN @IsPrimaryProfile = 1
                    AND ep.IsPrimary = 1
                   THEN
                   CAST(1 AS BIT)
               WHEN @IsPrimaryProfile = 0
                    AND ep.IsPrimary = 0
                   THEN
                   CAST(1 AS BIT)
               ELSE
                   CAST(0 AS BIT)
           END     AS IsSelected
FROM
           identities.QUser         AS p
INNER JOIN contacts.Membership       AS rm
           ON rm.UserIdentifier = p.UserIdentifier

INNER JOIN contacts.QGroup          AS r
           ON r.GroupIdentifier = rm.GroupIdentifier

INNER JOIN contacts.Membership AS m
           ON m.UserIdentifier = p.UserIdentifier

LEFT JOIN  custom_cmds.UserProfile   AS ep
           ON ep.UserIdentifier = p.UserIdentifier
              AND ep.DepartmentIdentifier = m.GroupIdentifier
              AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier
WHERE
           m.GroupIdentifier = @DepartmentIdentifier
           AND m.MembershipType = 'Department'
           AND r.GroupName = @PermissionListGroupName
           AND p.UtcArchived IS NULL
ORDER BY
           p.FullName;";

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("DepartmentIdentifier", department),
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier),
                new SqlParameter("PermissionListGroupName", CmdsRole.Workers),
                new SqlParameter("IsPrimaryProfile", isPrimaryProfile)
                );
        }

        public static DataTable SelectPersons(CmdsPersonFilter filter, Guid organization)
        {
            const string query = @"
SELECT cmdsPerson.*
      ,P.PhoneWork AS Phone
      ,cmdsPerson.Email AS EmailWork
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
  FROM identities.QUser AS cmdsPerson
  LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
  WHERE cmdsPerson.AccessGrantedToCmds = 1 AND {0}
  ORDER BY FullName;";

            var where = CreateWhereForFilter(filter, null);
            var curQuery = string.Format(query, where);

            return DatabaseHelper.CreateDataTable(curQuery, GetParametersForFilter(filter, null, organization).ToArray());
        }

        public static DataTable SelectPersonsWithCertificationInfo(CmdsPersonFilter filter, Guid achievementIdentifier, Guid organization)
        {
            const string query = @"
SELECT cmdsPerson.*
      ,P.PhoneWork AS Phone
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
      ,CAST(
         CASE WHEN ec.AchievementIdentifier IS NOT NULL THEN 1
              ELSE 0
         END AS bit
       ) AS IsAssigned
      ,ec.CredentialStatus AS ResourceValidationStatus
      ,ec.AuthorityReference AS ResourceNumber
      ,CAST(ec.CredentialExpirationExpected AS DATETIME) AS ResourceExpirationDate
      ,CAST(ec.CredentialGranted AS DATETIME) AS ResourceDateCompleted
FROM
    identities.QUser AS cmdsPerson
    LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
    LEFT JOIN custom_cmds.VCmdsCredential AS ec ON ec.UserIdentifier = cmdsPerson.UserIdentifier
                                            AND ec.AchievementIdentifier = @AchievementIdentifier
WHERE
    cmdsPerson.AccessGrantedToCmds = 1
    AND {0}
ORDER BY
    FullName;
";

            var where = CreateWhereForFilter(filter, null);
            var curQuery = string.Format(query, where);

            var parameters = GetParametersForFilter(filter, null, organization);
            parameters.Add(new SqlParameter("AchievementIdentifier", achievementIdentifier));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectAchievementUploadsForPersons(CmdsPersonFilter filter, Guid achievementIdentifier)
        {
            const string query = @"
SELECT
    cmdsPerson.UserIdentifier
   ,cmdsPerson.UserIdentifier
   ,Upload.UploadIdentifier
   ,Upload.ContainerIdentifier
   ,Upload.[Name]
   ,Upload.Title
   ,Upload.[Description]
FROM
    identities.QUser AS cmdsPerson
    INNER JOIN custom_cmds.VCmdsCredential ON VCmdsCredential.UserIdentifier = cmdsPerson.UserIdentifier
    INNER JOIN resources.Upload ON Upload.ContainerIdentifier = VCmdsCredential.CredentialIdentifier
WHERE
    {0}
    AND VCmdsCredential.AchievementIdentifier = @AchievementIdentifier
    AND cmdsPerson.AccessGrantedToCmds = 1
ORDER BY
    cmdsPerson.UserIdentifier;";

            var where = CreateWhereForFilter(filter, null);
            var curQuery = string.Format(query, where);

            var parameters = GetParametersForFilter(filter, null, null);
            parameters.Add(new SqlParameter("AchievementIdentifier", achievementIdentifier));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectPersonsWithPolicyInfo(CmdsPersonFilter filter, Guid[] achievements, Guid organization)
        {
            const string query = @"
SELECT cmdsPerson.*
      ,P.PhoneWork AS Phone
      ,cmdsPerson.Email AS EmailWork
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
      ,CAST(
         CASE WHEN {0} = (
                SELECT COUNT(*)
                  FROM custom_cmds.VCmdsCredential
                  WHERE AchievementIdentifier IN ({1})
                        AND UserIdentifier = cmdsPerson.UserIdentifier
              ) THEN 1
              ELSE 0
         END AS bit
       )AS IsSelected
      ,(SELECT CAST(MAX(CredentialGranted) AS DATETIME)
          FROM custom_cmds.VCmdsCredential
          WHERE UserIdentifier = cmdsPerson.UserIdentifier
                AND AchievementIdentifier IN ({1})
       ) AS DateCompleted
  FROM identities.QUser AS cmdsPerson
  LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
  WHERE AccessGrantedToCmds = 1 AND {2}
  ORDER BY FullName";

            var count = achievements.IsEmpty() ? "NULL" : achievements.Length.ToString(Cultures.Default);
            var achievementTextList = achievements.IsEmpty() ? "NULL" : CsvConverter.ConvertListToCsvText(achievements, true);
            var where = CreateWhereForFilter(filter, null);
            var curQuery = string.Format(query, count, achievementTextList, where);

            return DatabaseHelper.CreateDataTable(curQuery, GetParametersForFilter(filter, null, organization).ToArray());
        }

        public static DataTable SelectPersonsWithManagerInfo(CmdsPersonFilter filter, Guid managerId, RelationCategory relationCategory, Guid organization)
        {
            const string query = @"
SELECT cmdsPerson.*
      ,P.PhoneWork AS Phone
      ,cmdsPerson.Email AS EmailWork
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
      ,CAST(
         CASE WHEN r.ToUserIdentifier IS NOT NULL THEN 1
              ELSE 0
         END AS bit
       )AS IsAssigned
  FROM identities.QUser AS cmdsPerson
       LEFT JOIN identities.UserConnection AS r
         ON r.ToUserIdentifier = cmdsPerson.UserIdentifier
            AND r.FromUserIdentifier = @ManagerID
            AND r.{1}
  LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
  WHERE cmdsPerson.AccessGrantedToCmds = 1 AND {0}
  ORDER BY FullName;";

            var where = CreateWhereForFilter(filter, null);
            var connectionWhere = GetRelationCategoryFilter(relationCategory);
            var curQuery = string.Format(query, where, connectionWhere);

            var parameters = GetParametersForFilter(filter, null, organization);
            parameters.Add(new SqlParameter("ManagerID", managerId));
            parameters.Add(new SqlParameter("RelationCategory", relationCategory));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountPeople(CmdsPersonFilter filter)
        {
            var where = CreateWhereForFilter(filter, null);

            var query = string.Format(@"
SELECT COUNT(*)
  FROM identities.QUser AS cmdsPerson
  WHERE {0}", where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForFilter(filter, null, null).ToArray()).FirstOrDefault();
        }

        public static DataTable SelectOrganizationsForSelector(int userKey, bool showArchiveIfEmpty)
        {
            const string query = @"
SELECT     DISTINCT
           T.OrganizationIdentifier  AS Value
         , T.CompanyName AS Text
         , T.OrganizationCode AS Code
FROM
           accounts.QOrganization AS T
INNER JOIN contacts.QPerson    AS P
           ON P.OrganizationIdentifier = T.OrganizationIdentifier
WHERE
           P.UserIdentifier = @UserIdentifier
ORDER BY
           T.CompanyName;
";

            var table = DatabaseHelper.CreateDataTable(query, new SqlParameter("UserIdentifier", userKey));

            if (showArchiveIfEmpty && table.Rows.Count > 1)
            {
                for (int i = table.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow row = table.Rows[i];
                    if ((string)row["Code"] == "archive")
                        row.Delete();
                }
                table.AcceptChanges();
            }

            return table;
        }

        public static List<Department> SelectGroupsByFromId(Guid division)
        {
            const string query = @"
SELECT Department.*
  FROM identities.Department
  WHERE Department.DivisionIdentifier = @DivisionIdentifier
  ORDER BY Department.DepartmentName;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<Department>(query, new SqlParameter("DivisionIdentifier", division)).ToList();
        }

        public static DataTable SelectForSchoolSelector()
        {
            const string query = @"
SELECT
    G.GroupName AS Value
  , G.GroupName AS Text
FROM
    contacts.QGroup AS G INNER JOIN accounts.QOrganization AS T ON T.OrganizationIdentifier = G.OrganizationIdentifier
WHERE
    T.OrganizationCode = 'cmds' AND G.GroupLabel = 'School'
ORDER BY
    G.GroupName";

            return DatabaseHelper.CreateDataTable(query);
        }

        public static DataTable SelectPersonDepartments(Guid organizationId, Guid personId, string[] roleType)
        {
            const string query = @"
SELECT
    department.*
   ,NULL AS ArchiveDate
   ,NULL AS DisabledBy
   ,NULL AS UtcDisabled
FROM
    identities.Department
    INNER JOIN contacts.Membership AS m ON m.GroupIdentifier = department.DepartmentIdentifier
WHERE
    m.UserIdentifier = @UserIdentifier
    AND department.OrganizationIdentifier = @OrganizationIdentifier
    {0}
ORDER BY
    Department.DepartmentName;";

            var where = roleType.IsNotEmpty()
                ? string.Format(" AND m.MembershipType IN ({0})", CsvConverter.ConvertListToCsvText(roleType, true))
                : null;

            var curQuery = string.Format(query, where);

            return DatabaseHelper.CreateDataTable(curQuery, new SqlParameter("UserIdentifier", personId), new SqlParameter("OrganizationIdentifier", organizationId));
        }

        public static DataTable SelectPersonOrganizations(Guid userKey)
        {
            const string query = @"
SELECT
    Organization.OrganizationIdentifier
   ,Organization.CompanyTitle
   ,NULL AS DisabledBy
   ,NULL AS UtcDisabled
  FROM accounts.QOrganization AS Organization
  WHERE Organization.OrganizationIdentifier IN (
        SELECT departments.OrganizationIdentifier
          FROM contacts.Membership AS m
          INNER JOIN identities.Department AS departments ON departments.DepartmentIdentifier = m.GroupIdentifier
          WHERE m.UserIdentifier = @UserIdentifier
      )
  ORDER BY CompanyTitle
";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("UserIdentifier", userKey));
        }

        public static DataTable SelectForActiveCompanySelector(int? personId)
        {
            const string query = @"
SELECT
    OrganizationIdentifier
   ,companyTitle AS CompanyName
   ,NULL AS CountryName
FROM
    accounts.QOrganization
WHERE
    AccountClosed IS NULL
    AND PlatformCode = 'CMDS'
    AND (
        @UserIdentifier IS NULL
        OR OrganizationIdentifier IN (
            SELECT
                departments.OrganizationIdentifier
            FROM
                contacts.Membership AS m
                INNER JOIN identities.Department AS departments ON departments.DepartmentIdentifier = m.GroupIdentifier
            WHERE
                m.UserIdentifier = @UserIdentifier
        )
      )
ORDER BY
    CountryName
   ,CompanyName;
";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("UserIdentifier", personId));
        }

        public static DataTable SelectManagersForGroupCompetencySummaryViewer(Guid organizationId)
        {
            const String query = @"
SELECT     DISTINCT
           person.UserIdentifier
         , person.FullName
         , ValueHelper.WorkersCount
         , UserRole.*
FROM
           accounts.QOrganization AS Organization
INNER JOIN identities.Department  AS department
           ON Organization.OrganizationIdentifier = department.OrganizationIdentifier

INNER JOIN contacts.Membership AS departmentMembership
           ON department.DepartmentIdentifier = departmentMembership.GroupIdentifier

INNER JOIN contacts.QPerson AS person
           ON person.UserIdentifier = departmentMembership.UserIdentifier
           AND person.OrganizationIdentifier = Organization.OrganizationIdentifier

INNER JOIN custom_cmds.UserRole
           ON UserRole.UserIdentifier = person.UserIdentifier
              AND UserRole.GroupName = 'CMDS Managers'
OUTER APPLY
           (
               SELECT
                   COUNT(*) AS WorkersCount
               FROM
                   identities.QUser AS p2
               WHERE
                   p2.UserIdentifier <> person.UserIdentifier
                   AND p2.UserIdentifier IN
                           (
                               SELECT
                                   ToUserIdentifier
                               FROM
                                   identities.UserConnection
                               WHERE
                                   FromUserIdentifier = person.UserIdentifier
                                   AND
                                       (
                                           IsManager = 1
                                           OR IsSupervisor = 1
                                       )
                           )
                   AND p2.UserIdentifier IN
                           (
                               SELECT
                                          m.UserIdentifier
                               FROM
                                          contacts.Membership AS m
                               INNER JOIN identities.Department     AS dep
                                          ON dep.DepartmentIdentifier = m.GroupIdentifier
                               WHERE
                                          dep.OrganizationIdentifier = @OrganizationIdentifier
                           )
           )                 AS ValueHelper
WHERE
           Organization.OrganizationIdentifier = @OrganizationIdentifier
           AND ValueHelper.WorkersCount > 0
ORDER BY
           person.FullName;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("OrganizationIdentifier", organizationId));
        }

        public static DataTable SelectManagerWorkersForGroupCompetencySummaryViewer(Guid managerId, Guid organizationId)
        {
            const String query = @"
SELECT p.UserIdentifier
      ,p.FullName
      ,ValueHelper.WorkersCount
  FROM contacts.QPerson AS p
       OUTER APPLY (
         SELECT COUNT(*) AS WorkersCount
           FROM identities.QUser AS p2
           WHERE p2.UserIdentifier <> p.UserIdentifier
             AND p2.UserIdentifier IN (
                 SELECT ToUserIdentifier
                   FROM identities.UserConnection
                   WHERE FromUserIdentifier = p.UserIdentifier
                     AND (IsManager = 1 OR IsSupervisor = 1)
               )
             AND p2.UserIdentifier IN (
               SELECT m.UserIdentifier
                 FROM contacts.Membership AS m
                      INNER JOIN identities.Department AS dep
                        ON dep.DepartmentIdentifier = m.GroupIdentifier
                 WHERE dep.OrganizationIdentifier = @OrganizationIdentifier
             )           
       ) AS ValueHelper
  WHERE p.OrganizationIdentifier = @OrganizationIdentifier
    AND p.UserIdentifier <> @ManagerID
    AND p.UserIdentifier IN (
        SELECT ToUserIdentifier
          FROM identities.UserConnection
          WHERE FromUserIdentifier = @ManagerID
            AND (IsManager = 1 OR IsSupervisor = 1)
      )
    AND p.UserIdentifier IN (
      SELECT m.UserIdentifier
        FROM contacts.Membership AS m
             INNER JOIN identities.Department AS dep
               ON dep.DepartmentIdentifier = m.GroupIdentifier
        WHERE dep.OrganizationIdentifier = @OrganizationIdentifier
    )
    AND ValueHelper.WorkersCount > 0
  ORDER BY FullName;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ManagerID", managerId), new SqlParameter("OrganizationIdentifier", organizationId));
        }

        #endregion

        #region SelectCompanySearchResults

        public static DataTable SelectSearchResults(CompanyFilter filter)
        {
            var where = CreateWhereForCompanySelector(filter);

            var sortExpression = "CompanyTitle";

            var withSortExpression = sortExpression
                .Replace("CompanyTitle", "o.CompanyTitle")
                .Replace("CompetencyCount", @"
                        (
                            SELECT COUNT(*)
                              FROM custom_cmds.VCmdsCompetencyOrganization
                              WHERE OrganizationIdentifier = o.OrganizationIdentifier
                        )")
                .Replace("ProfileCount", @"
                        (
                            SELECT COUNT(*)
                              FROM custom_cmds.VCmdsProfileOrganization
                              WHERE OrganizationIdentifier = o.OrganizationIdentifier
                        )")
                ;

            var query = string.Format(@"
WITH OrderedCompanies AS (
  SELECT
         o.OrganizationIdentifier
        ,o.CompanyTitle
        ,(SELECT COUNT(*)
            FROM custom_cmds.VCmdsCompetencyOrganization
            WHERE OrganizationIdentifier = o.OrganizationIdentifier
         ) AS CompetencyCount
        ,(SELECT COUNT(*)
            FROM custom_cmds.VCmdsProfileOrganization
            WHERE OrganizationIdentifier = o.OrganizationIdentifier
         ) AS ProfileCount
        ,(SELECT COUNT(*) FROM identities.Department WHERE OrganizationIdentifier = o.OrganizationIdentifier) AS DepartmentCount
        ,(
            SELECT COUNT(DISTINCT p.UserIdentifier)
            FROM
                identities.QUser AS p
                INNER JOIN contacts.Membership AS m ON m.UserIdentifier = p.UserIdentifier
                INNER JOIN identities.Department AS dep ON dep.DepartmentIdentifier = m.GroupIdentifier
            WHERE
                p.AccessGrantedToCmds = 1
                AND dep.OrganizationIdentifier = o.OrganizationIdentifier
        ) AS PersonCount
        ,(SELECT COUNT(*) FROM achievements.TAchievementOrganization WHERE OrganizationIdentifier = o.OrganizationIdentifier) AS AchievementCount
        ,ROW_NUMBER() OVER(ORDER BY {2}) AS RowNumber
  FROM accounts.QOrganization AS o
  WHERE {0}
)
SELECT *
  FROM OrderedCompanies
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY {1}", where, sortExpression, withSortExpression);

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForCompanySelector(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static int CountSearchResults(CompanyFilter filter)
        {
            var where = CreateWhereForCompanySelector(filter);

            var query = string.Format(@"
SELECT COUNT(*)
  FROM accounts.QOrganization AS o
  WHERE {0}", where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForCompanySelector(filter).ToArray()).FirstOrDefault();
        }

        #endregion

        #region SelectCompaniesForSelector

        private const string QueryCompaniesForSelector = @"
WITH Items AS (
    SELECT o.OrganizationIdentifier AS Value
          ,o.CompanyTitle AS Text
          ,ROW_NUMBER() OVER(ORDER BY o.CompanyTitle) AS RowNumber
    FROM accounts.QOrganization AS o
    WHERE {0}
)
SELECT *
  FROM Items
  {1}
  ORDER BY RowNumber";

        public static DataTable SelectCompaniesForSelector(CompanyFilter filter)
        {
            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var where = CreateWhereForCompanySelector(filter);
            var curQuery = string.Format(QueryCompaniesForSelector, where, "WHERE RowNumber BETWEEN @StartRow AND @EndRow");

            var parameters = GetParametersForCompanySelector(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectCompaniesForSelector(IEnumerable<Guid> ids)
        {
            var where = $"o.OrganizationIdentifier IN ({CsvConverter.ConvertListToCsvText(ids, true)})";
            var curQuery = string.Format(QueryCompaniesForSelector, where, null);

            return DatabaseHelper.CreateDataTable(curQuery);
        }

        public static int CountCompaniesForSelector(CompanyFilter filter)
        {
            const string query = "SELECT CAST(COUNT(*) AS INT) FROM accounts.QOrganization AS o WHERE {0}";

            var where = CreateWhereForCompanySelector(filter);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForCompanySelector(filter).ToArray()).FirstOrDefault();
        }

        #endregion

        #region Company filter

        private static string CreateWhereForCompanySelector(CompanyFilter filter)
        {
            var where = new StringBuilder();
            where.Append("1 = 1");

            if (filter.UserIdentifier.HasValue)
            {
                where.Append(@"
                    AND o.OrganizationIdentifier IN (
                          SELECT department.OrganizationIdentifier
                            FROM contacts.Membership AS m
                            INNER JOIN identities.Department
                              ON department.DepartmentIdentifier = m.GroupIdentifier
                            WHERE m.UserIdentifier = @UserIdentifier
                        )
                    ");
            }

            if (filter.CompetencyStandardIdentifier.HasValue)
            {
                where.Append(@"
                    AND o.OrganizationIdentifier IN (
                        SELECT department.OrganizationIdentifier
                          FROM identities.Department
                          INNER JOIN contacts.Membership AS m
                            ON department.DepartmentIdentifier = m.GroupIdentifier
                          INNER JOIN identities.QUser p
                            ON p.UserIdentifier = m.UserIdentifier
                          INNER JOIN custom_cmds.UserCompetency c
                            ON c.UserIdentifier = p.UserIdentifier
                          WHERE c.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                      )");
            }

            if (filter.AchievementForEmployeeId.HasValue)
            {
                where.Append(@"
                    AND o.OrganizationIdentifier IN (
                        SELECT department.OrganizationIdentifier
                          FROM identities.Department
                          INNER JOIN contacts.Membership AS m
                            ON department.DepartmentIdentifier = m.GroupIdentifier
                          INNER JOIN identities.QUser p
                            ON p.UserIdentifier = m.UserIdentifier
                          INNER JOIN custom_cmds.VCmdsCredential c
                            ON c.UserIdentifier = p.UserIdentifier
                          WHERE c.AchievementIdentifier = @AchievementForEmployeeID
                      )");
            }

            if (filter.Name.IsNotEmpty())
                where.Append(" AND o.CompanyTitle LIKE @Name");

            if (filter.RequireMembershipForUserEmail.IsNotEmpty())
            {
                where.Append(@"
                     AND o.OrganizationIdentifier IN ( SELECT
                        d.OrganizationIdentifier
                     FROM
                        contacts.Membership AS m
                        INNER JOIN identities.Department AS d ON m.GroupIdentifier = d.DepartmentIdentifier
                        INNER JOIN identities.QUser u ON m.UserIdentifier = u.UserIdentifier
                     WHERE
                        u.Email = @UserName
                    )
                    ");
            }

            switch (filter.Archived)
            {
                case InclusionType.Exclude:
                    where.Append(" AND o.AccountClosed IS NULL");
                    break;
                case InclusionType.Only:
                    where.Append(" AND o.AccountClosed IS NOT NULL");
                    break;
            }

            if (filter.IncludeOrganizationIdentifiers.IsNotEmpty())
            {
                where.AppendFormat(
                    " AND o.OrganizationIdentifier IN ({0})",
                    CsvConverter.ConvertListToCsvText(filter.IncludeOrganizationIdentifiers, true));
            }

            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForCompanySelector(CompanyFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (filter.UserIdentifier.HasValue)
                parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (filter.CompetencyStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("CompetencyStandardIdentifier", filter.CompetencyStandardIdentifier));

            if (filter.AchievementForEmployeeId.HasValue)
                parameters.Add(new SqlParameter("AchievementForEmployeeID", filter.AchievementForEmployeeId));

            if (filter.Name.IsNotEmpty())
                parameters.Add(new SqlParameter("Name", string.Format("%{0}%", filter.Name)));

            if (filter.RequireMembershipForUserEmail.IsNotEmpty())
                parameters.Add(new SqlParameter("UserName", filter.RequireMembershipForUserEmail));

            return parameters;
        }

        #endregion

        #region SelectByDistrictFilter

        public static int? SelectDistrictIDForDepartment(int departmentKey)
        {
            const string query = @"
SELECT g.DivisionIdentifier
  FROM identities.Department AS g
  WHERE g.DepartmentIdentifier = @DepartmentIdentifier
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, new SqlParameter("DepartmentIdentifier", departmentKey)).FirstOrDefault();
        }

        public static DataTable SelectDistrictsForSelector(Guid organizationId)
        {
            const string query = @"
SELECT
    DivisionIdentifier AS [Value]
   ,DivisionName AS [Text]
FROM
    identities.Division AS o
{0}
ORDER BY
    DivisionName";

            var where = CreateWhereForDistrictFilter();
            var curQuery = string.Format(query, where);

            return DatabaseHelper.CreateDataTable(curQuery, GetParametersForDistrictFilter(organizationId).ToArray());
        }

        public static DataTable SelectByDistrictFilter(Guid organizationId, Paging paging)
        {
            const string query = @"
WITH Districts AS
(
  SELECT
      o.DivisionIdentifier
     ,o.DivisionName
     ,COUNT(r.DepartmentIdentifier) AS DepartmentCount
     ,ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumber
    FROM identities.Division AS o
    LEFT JOIN identities.Department AS r ON r.DivisionIdentifier = o.DivisionIdentifier
    {0}
    GROUP BY o.DivisionIdentifier, o.DivisionName
)
SELECT *
  FROM Districts
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY RowNumber
";

            var where = CreateWhereForDistrictFilter();

            var sortExpression = "DivisionName";

            var withSortExpression = sortExpression
                .Replace("DivisionName", "o.DivisionName")
                .Replace("DepartmentCount", "COUNT(r.DepartmentIdentifier)");

            var curQuery = string.Format(query, where, withSortExpression);

            var (startRow, endRow) = paging != null ? paging.ToStartEnd() : (0, 0);

            var parameters = GetParametersForDistrictFilter(organizationId);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountByDistrictFilter(Guid organizationId)
        {
            const string query = @"
SELECT COUNT(*)
  FROM identities.Division AS o
  {0}
";

            var where = CreateWhereForDistrictFilter();
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForDistrictFilter(organizationId).ToArray()).FirstOrDefault();
        }

        private static string CreateWhereForDistrictFilter()
        {
            var where = new StringBuilder();
            where.Append("WHERE o.OrganizationIdentifier = @OrganizationIdentifier");
            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForDistrictFilter(Guid organizationId)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("OrganizationIdentifier", organizationId));

            return parameters;
        }

        #endregion

        #region Select departments

        public static DataTable SelectDepartmentsWithCounts(DepartmentFilter filter)
        {
            const string query = @"
WITH Departments AS
(
    SELECT department.*
          ,NULL AS ArchiveDate
          ,NULL AS DisabledBy
          ,NULL AS UtcDisabled
          ,Division.DivisionName
          ,(SELECT COUNT(DISTINCT CompetencyStandardIdentifier)
              FROM custom_cmds.DepartmentProfileCompetency
              WHERE DepartmentIdentifier = department.DepartmentIdentifier
           ) AS CompetencyCount
          ,(SELECT COUNT(*)
              FROM custom_cmds.DepartmentProfile
              WHERE DepartmentIdentifier = department.DepartmentIdentifier
           ) AS ProfileCount
          ,(SELECT COUNT(*) FROM contacts.Membership WHERE Membership.GroupIdentifier = Department.DepartmentIdentifier) as UserCount
          ,ROW_NUMBER() OVER (ORDER BY Department.DepartmentName) AS RowNumber
      FROM identities.Department
           LEFT JOIN identities.Division ON Department.DivisionIdentifier = Division.DivisionIdentifier
      {0}
)
SELECT * FROM Departments AS Department
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY RowNumber";

            var where = CreateWhereForDepartmentFilter(filter);
            var curQuery = string.Format(query, where);

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForDepartmentFilter(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectDepartments(DepartmentFilter filter, string sortExpression = null)
        {
            const string query = @"
SELECT
    department.*
   ,NULL AS ArchiveDate
   ,Department.DepartmentDescription
   ,NULL AS DisabledBy
   ,NULL AS UtcDisabled
   ,Division.DivisionIdentifier
   ,Division.DivisionName
   ,ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumber
  FROM identities.Department
  LEFT JOIN identities.Division ON Department.DivisionIdentifier = Division.DivisionIdentifier
  {0}
  ORDER BY RowNumber
";

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "Department.DepartmentName";
            else
            {
                sortExpression = sortExpression
                    .Replace("DepartmentName", "Department.DepartmentName")
                    .Replace("DivisionName", "Division.DivisionName")
                    .Replace("DivisionIdentifier", "Division.DivisionIdentifier")
                    ;
            }

            var where = CreateWhereForDepartmentFilter(filter);
            var curQuery = string.Format(query, where, sortExpression);

            return DatabaseHelper.CreateDataTable(curQuery, 60, GetParametersForDepartmentFilter(filter).ToArray());
        }

        public static int CountDepartments(DepartmentFilter filter, string searchText = null)
        {
            const string query = @"SELECT COUNT(*) FROM identities.Department {0}";

            var where = CreateWhereForDepartmentFilter(filter, searchText);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForDepartmentFilter(filter, searchText).ToArray()).FirstOrDefault();
        }

        private const string QueryDepartmentsForSelector = @"
WITH Departments AS (
    SELECT DepartmentIdentifier
          ,DepartmentName
          ,ROW_NUMBER() OVER(ORDER BY DepartmentName) AS RowNumber
    FROM identities.Department
    {0}
)
SELECT *
  FROM Departments
  {1}
  ORDER BY RowNumber";

        private const string QueryDepartmentsWithDivisionForSelector = @"
WITH Departments AS (
    SELECT Department.DepartmentIdentifier
          ,Department.DepartmentName
          ,Division.DivisionIdentifier
          ,Division.DivisionName
          ,ROW_NUMBER() OVER(ORDER BY Division.DivisionName, Department.DepartmentName) AS RowNumber
    FROM identities.Department
    LEFT JOIN identities.Division ON Division.DivisionIdentifier = Department.DivisionIdentifier
    {0}
)
SELECT *
  FROM Departments
  {1}
  ORDER BY RowNumber";

        public static DataTable SelectDepartmentsForSelector(DepartmentFilter filter, string searchText, bool includeDivision)
        {
            var where = CreateWhereForDepartmentFilter(filter, searchText);
            var curQuery = includeDivision ? QueryDepartmentsWithDivisionForSelector : QueryDepartmentsForSelector;
            curQuery = string.Format(curQuery, where, "WHERE RowNumber BETWEEN @StartRow AND @EndRow");

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForDepartmentFilter(filter, searchText);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectDepartmentsForSelector(IEnumerable<Guid> ids)
        {
            var where = $"WHERE DepartmentIdentifier IN ({CsvConverter.ConvertListToCsvText(ids, true)})";
            var curQuery = string.Format(QueryDepartmentsForSelector, where, null);

            return DatabaseHelper.CreateDataTable(curQuery);
        }

        private static string CreateWhereForDepartmentFilter(DepartmentFilter filter, string searchText = null)
        {
            var where = new StringBuilder();

            where.Append("WHERE Department.OrganizationIdentifier = @OrganizationIdentifier");

            if (filter.ExcludeUserIdentifier.HasValue)
            {
                where.Append(@"
                    AND Department.DepartmentIdentifier NOT IN (
                          SELECT Membership.GroupIdentifier
                            FROM contacts.Membership
                            WHERE UserIdentifier = @ExcludeUserIdentifier
                      )");
            }

            if (filter.CompetencyStandardIdentifier.HasValue)
            {
                where.Append(@"
                    AND department.DepartmentIdentifier IN (
                        SELECT m.GroupIdentifier
                          FROM contacts.Membership m
                          INNER JOIN identities.QUser p
                            ON p.UserIdentifier = m.UserIdentifier
                          INNER JOIN custom_cmds.UserCompetency c
                            ON c.UserIdentifier = p.UserIdentifier
                          WHERE c.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                      )");
            }

            if (filter.AchievementForEmployeeId.HasValue)
            {
                where.Append(@"
                    AND department.DepartmentIdentifier IN (
                        SELECT m.GroupIdentifier
                          FROM contacts.Membership m
                          INNER JOIN identities.QUser p
                            ON p.UserIdentifier = m.UserIdentifier
                          INNER JOIN custom_cmds.VCmdsCredential c
                            ON c.UserIdentifier = p.UserIdentifier
                          WHERE c.AchievementIdentifier = @AchievementForEmployeeID
                      )");
            }

            if (filter.UserIdentifier.HasValue)
            {
                where.Append(@"
                    AND department.DepartmentIdentifier IN (
                          SELECT GroupIdentifier
                            FROM contacts.Membership
                            WHERE UserIdentifier = @UserIdentifier
                      ");

                if (filter.RoleType.IsNotEmpty())
                    where.AppendFormat(" AND MembershipType IN ({0})", CsvConverter.ConvertListToCsvText(filter.RoleType, true));

                where.Append(")");
            }

            if (filter.ProfileStandardIdentifier.HasValue)
            {
                where.Append(@"
                    AND department.DepartmentIdentifier IN (
                        SELECT DepartmentIdentifier
                          FROM custom_cmds.DepartmentProfile 
                          WHERE ProfileStandardIdentifier = @ProfileStandardIdentifier
                      )
                    ");
            }

            if (filter.DepartmentLabel.HasValue())
                where.Append(" AND department.DepartmentLabel = @DepartmentLabel");

            if (searchText.HasValue())
                where.Append(" AND DepartmentName LIKE @SearchText");

            if (filter.DepartmentName.HasValue())
                where.Append(" AND DepartmentName LIKE @DepartmentName");

            if (filter.DivisionIdentifier.HasValue)
                where.Append(" AND department.DivisionIdentifier = @DivisionIdentifier");

            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForDepartmentFilter(DepartmentFilter filter, string searchText = null)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            if (filter.ExcludeUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ExcludeUserIdentifier", filter.ExcludeUserIdentifier));

            if (filter.CompetencyStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("CompetencyStandardIdentifier", filter.CompetencyStandardIdentifier));

            if (filter.AchievementForEmployeeId.HasValue)
                parameters.Add(new SqlParameter("AchievementForEmployeeID", filter.AchievementForEmployeeId));

            if (filter.UserIdentifier.HasValue)
                parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (filter.ProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileStandardIdentifier", filter.ProfileStandardIdentifier));

            if (filter.DepartmentLabel.HasValue())
                parameters.Add(new SqlParameter("DepartmentLabel", filter.DepartmentLabel));

            if (searchText.HasValue())
                parameters.Add(new SqlParameter("SearchText", string.Format("%{0}%", searchText)));

            if (filter.DepartmentName.HasValue())
                parameters.Add(new SqlParameter("DepartmentName", string.Format("%{0}%", filter.DepartmentName)));

            if (filter.DivisionIdentifier.HasValue)
                parameters.Add(new SqlParameter("DivisionIdentifier", filter.DivisionIdentifier));

            return parameters;
        }

        #endregion

        #region SelectPersonGroups

        public static DataTable SelectDepartmentUsers(DepartmentUserFilter filter)
        {
            const string query = @"
WITH OrderedGroups AS (
  SELECT department.OrganizationIdentifier
        ,department.DepartmentIdentifier
        ,organization.CompanyTitle AS CompanyName
        ,department.DepartmentName
        ,m.MembershipType AS RoleType
        ,ISNULL(e.IsPrimary, CAST(0 AS BIT)) AS IsPrimary
        ,ROW_NUMBER() OVER ( ORDER BY {2} ) AS RowNumber
    FROM contacts.Membership AS m
         INNER JOIN identities.Department
           ON department.DepartmentIdentifier = m.GroupIdentifier
         INNER JOIN accounts.QOrganization AS organization
           ON organization.OrganizationIdentifier = department.OrganizationIdentifier
         LEFT JOIN custom_cmds.UserProfile AS e
           ON e.DepartmentIdentifier = department.DepartmentIdentifier
              AND e.UserIdentifier = @UserIdentifier
              AND e.IsPrimary = 1
    {0}
)
SELECT *
  FROM OrderedGroups
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY {1}";

            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "CompanyName, DepartmentName";

            var withSortExpression = sortExpression
                .Replace("CompanyName", "organization.CompanyTitle")
                .Replace("DepartmentName", "department.DepartmentName");

            var where = CreateWhereForPersonGroup(filter);
            var curQuery = string.Format(query, where, sortExpression, withSortExpression);

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForPersonGroup(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountDepartmentUsers(DepartmentUserFilter filter)
        {
            const string query = @"
SELECT COUNT(*)
  FROM contacts.Membership AS m WITH(NOLOCK)
       INNER JOIN identities.Department WITH(NOLOCK)
         ON department.DepartmentIdentifier = m.GroupIdentifier
       INNER JOIN accounts.QOrganization as organization WITH(NOLOCK)
         ON organization.OrganizationIdentifier = department.OrganizationIdentifier
  {0}";

            var where = CreateWhereForPersonGroup(filter);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForPersonGroup(filter).ToArray()).FirstOrDefault();
        }

        private static List<SqlParameter> GetParametersForPersonGroup(DepartmentUserFilter filter)
        {
            var parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (filter.SearchText.IsNotEmpty())
                parameters.Add(new SqlParameter("SearchText", string.Format("%{0}%", filter.SearchText)));

            return parameters;
        }

        private static string CreateWhereForPersonGroup(DepartmentUserFilter filter)
        {
            var where = new StringBuilder();
            where.Append("WHERE m.UserIdentifier = @UserIdentifier");

            if (filter.SearchText.IsNotEmpty())
                where.Append(" AND (department.DepartmentName LIKE @SearchText OR Organization.CompanyTitle LIKE @SearchText)");

            if (filter.RoleType.IsNotEmpty())
            {
                var listText = CsvConverter.ConvertListToCsvText(filter.RoleType, true);

                where.AppendFormat(" AND m.MembershipType IN ({0})", listText);
            }

            return where.ToString();
        }

        #endregion

        #region SelectSearchResults (PersonFilter)

        public static DataTable SelectSearchResultsWithDepartment(CmdsPersonFilter filter, Guid department, Guid organization)
        {
            var where = CreateWhereForFilter(filter, null);

            var query = string.Format(@"
SELECT cmdsPerson.*
      ,P.PhoneWork AS Phone
      ,cmdsPerson.Email AS EmailWork
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
      ,CAST(
         CASE WHEN m.UserIdentifier IS NOT NULL THEN 1
              ELSE 0
         END AS bit
       ) AS IsSelected
  FROM identities.QUser AS cmdsPerson
  LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
       LEFT JOIN contacts.Membership AS m
         ON m.UserIdentifier = cmdsPerson.UserIdentifier
            AND m.GroupIdentifier = @DepartmentIdentifier
  WHERE {0}
  ORDER BY FullName;", where);

            var parameters = GetParametersForFilter(filter, null, organization);
            parameters.Add(new SqlParameter("DepartmentIdentifier", department));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static DataTable SelectSearchResultsWithProfiles(CmdsPersonFilter filter, PersonProfileFilter profileFilter, Guid organization)
        {
            const string query = @"
SELECT cmdsPerson.*
      ,P.FullName AS PersonFullName
      ,P.PhoneWork AS Phone
      ,cmdsPerson.Email AS EmailWork
      ,cmdsPerson.UtcArchived AS ArchiveDate
      ,NULL AS DisabledBy
      ,NULL AS UtcDisabled
      ,profiles.ProfileStandardIdentifier
      ,profiles.ProfileNumber
      ,ep.IsPrimary AS IsPrimaryProfile
      ,ep.CurrentStatus AS ProfileStatusName
  FROM identities.QUser AS cmdsPerson
       CROSS APPLY (
         SELECT ep.ProfileStandardIdentifier AS ProfileStandardIdentifier
           FROM custom_cmds.UserProfile AS ep
           WHERE ep.UserIdentifier = cmdsPerson.UserIdentifier
                 {0}
           GROUP BY ep.ProfileStandardIdentifier
       ) AS ep_max
       INNER JOIN custom_cmds.UserProfile AS ep ON ep.ProfileStandardIdentifier = ep_max.ProfileStandardIdentifier
                                                       AND ep.UserIdentifier = cmdsPerson.UserIdentifier
       INNER JOIN custom_cmds.[Profile] AS profiles ON profiles.ProfileStandardIdentifier = ep.ProfileStandardIdentifier
       LEFT JOIN contacts.QPerson AS P ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
  WHERE {1}
  ORDER BY PersonFullName, profiles.ProfileNumber";

            var profileWhere = new StringBuilder();

            if (profileFilter.OnlyComplianceRequired)
                profileWhere.Append(" AND ep.IsComplianceRequired = 1");

            if (profileFilter.ProfileStandardIdentifier.HasValue)
                profileWhere.Append(" AND ep.ProfileStandardIdentifier = @ProfileStandardIdentifier");

            if (filter.DepartmentIdentifier.HasValue)
                profileWhere.Append(" AND ep.DepartmentIdentifier = @DepartmentIdentifier");

            var where = CreateWhereForFilter(filter, null);

            if (profileFilter.OnlyPrimaryProfile)
                where += " AND ep.IsPrimary = 1";

            if (filter.DepartmentIdentifier.HasValue)
                where += " AND ep.DepartmentIdentifier = @DepartmentIdentifier";

            var curQuery = string.Format(query, profileWhere, where);

            var parameters = GetParametersForFilter(filter, null, organization);
            parameters.Add(new SqlParameter("ProfileStandardIdentifier", profileFilter.ProfileStandardIdentifier));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static DataTable SelectSearchResults(CmdsPersonFilter filter, Guid organization)
        {
            const string CompanyModeExpr = @"
CASE
  WHEN organization.CompanyTitle IS NULL
       AND EXISTS (
         SELECT *
           FROM contacts.Membership AS m WITH (NOLOCK)
                INNER JOIN identities.Department AS o WITH (NOLOCK)
                  ON o.DepartmentIdentifier = m.GroupIdentifier
           WHERE m.UserIdentifier = cmdsPerson.UserIdentifier
       )
    THEN 2
  WHEN organization.CompanyTitle IS NULL
    THEN 0
  ELSE 1
END
";

            var where = CreateWhereForFilter(filter, null);

            var sortExpression = "FullName";
            var withSortExpression = "cmdsPerson.FullName";

            string companyModeExpr, companyNameExpr, companyJoin;

            if (sortExpression.IndexOf("CompanyName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                companyModeExpr = CompanyModeExpr;
                companyNameExpr = "organization.CompanyTitle";

                companyJoin = @"
LEFT JOIN accounts.QOrganization AS organization
  ON (
    organization.OrganizationIdentifier = ALL (
      SELECT departments.OrganizationIdentifier
        FROM contacts.Membership AS m WITH (NOLOCK)
             INNER JOIN identities.Department AS departments WITH (NOLOCK)
               ON departments.DepartmentIdentifier = m.GroupIdentifier
        WHERE m.UserIdentifier = cmdsPerson.UserIdentifier
    )
    AND EXISTS (
      SELECT *
        FROM contacts.Membership AS m WITH (NOLOCK)
             INNER JOIN identities.Department AS departments WITH (NOLOCK)
               ON departments.DepartmentIdentifier = m.GroupIdentifier
        WHERE m.UserIdentifier = cmdsPerson.UserIdentifier
    )
  )
";
            }
            else
            {
                companyModeExpr = "NULL";
                companyNameExpr = "NULL";
                companyJoin = null;
            }

            var query = string.Format(@"
WITH OrderedPersons AS
(
  SELECT cmdsPerson.UserIdentifier
        ,cmdsPerson.FullName + (case when P.EmployeeType is null then '' else ' [' + P.EmployeeType + ']' end) as FullName
        ,cmdsPerson.Email
        ,(select cast(isnull(max(cast(Q.EmailEnabled as int)),0) as bit) from contacts.QPerson as Q  with (nolock) where Q.UserIdentifier = cmdsPerson.UserIdentifier) as EmailEnabled
        ,P.PhoneWork AS Phone
        ,P.LastAuthenticated
        ,cmdsPerson.UtcArchived AS Archived
        ,NULL AS DisabledBy
        ,NULL AS UtcDisabled
        ,CASE
           WHEN EXISTS (
                  SELECT *
                    FROM custom_cmds.Employment AS e WITH (NOLOCK)
                    WHERE e.UserIdentifier = cmdsPerson.UserIdentifier
                      AND (
                            e.DepartmentIdentifier IS NOT NULL
                        AND e.ProfileStandardIdentifier IS NOT NULL
                      )
                )
             THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT)
         END AS HasPrimaryProfile
        ,{3} AS CompanyMode
        ,{4} AS CompanyName
        ,0 AS Lists
        ,HomeAddress.City AS AddressCity
        ,HomeAddress.Province AS AddressProvince
        ,ROW_NUMBER() OVER(ORDER BY {2}) AS RowNumber
    FROM identities.QUser AS cmdsPerson WITH (NOLOCK)
    LEFT JOIN contacts.QPerson AS P WITH (NOLOCK) ON P.UserIdentifier = cmdsPerson.UserIdentifier AND P.OrganizationIdentifier = @PersonOrganizationIdentifier
    LEFT JOIN locations.[Address] AS HomeAddress WITH (NOLOCK) ON HomeAddress.AddressIdentifier = P.HomeAddressIdentifier
    {5}

    WHERE {0}
)
SELECT *
  FROM OrderedPersons
  WHERE RowNumber BETWEEN @StartRow AND @EndRow
  ORDER BY {1}", where, sortExpression, withSortExpression, companyModeExpr, companyNameExpr, companyJoin);

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForFilter(filter, null, organization);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static int CountSearchResults(CmdsPersonFilter filter)
        {
            var where = CreateWhereForFilter(filter, null);

            var query = string.Format(@"
SELECT COUNT(*)
  FROM identities.QUser AS cmdsPerson WITH (NOLOCK)
  WHERE {0}", where);



            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, GetParametersForFilter(filter, null, null).ToArray()).FirstOrDefault();
        }

        #endregion

        #region SelectForPersonSelector

        private const string QueryForPersonSelector = @"
WITH OrderedPersons AS (
  SELECT cmdsPerson.UserIdentifier AS [Value]
        ,cmdsPerson.FullName AS [Text]
        ,cmdsPerson.Email AS UserName
        ,ROW_NUMBER() OVER(ORDER BY cmdsPerson.FullName) AS RowNumber
    FROM identities.QUser AS cmdsPerson
    WHERE {0}
)
SELECT *
  FROM OrderedPersons
  {1}
  ORDER BY RowNumber
OPTION (OPTIMIZE FOR UNKNOWN)";

        public static DataTable SelectForPersonSelector(IEnumerable<Guid> ids)
        {
            var where = $"cmdsPerson.UserIdentifier IN ({CsvConverter.ConvertListToCsvText(ids, true)})";
            var curQuery = string.Format(QueryForPersonSelector, where, null);

            return DatabaseHelper.CreateDataTable(curQuery);
        }

        public static DataTable SelectForPersonSelector(CmdsPersonFilter filter, string searchText)
        {
            var where = CreateWhereForFilter(filter, searchText);
            var curQuery = string.Format(QueryForPersonSelector, where, "WHERE RowNumber BETWEEN @StartRow AND @EndRow");

            var (startRow, endRow) = filter?.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForFilter(filter, searchText, null);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int SelectCountForPersonSelector(CmdsPersonFilter filter, string searchText)
        {
            const string query = @"
/* SelectCountForPersonSelector */

SELECT COUNT(*)
  FROM identities.QUser AS cmdsPerson
  WHERE {0} OPTION (OPTIMIZE FOR UNKNOWN)";

            var where = CreateWhereForFilter(filter, searchText);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForFilter(filter, searchText, null).ToArray()).FirstOrDefault();

        }

        #endregion

        #region Filter helpers (PersonFilter)

        private static string GetRelationCategoryFilter(RelationCategory value)
        {
            switch (value)
            {
                case RelationCategory.Manager: return "IsManager = 1";
                case RelationCategory.Supervisor: return "IsSupervisor = 1";
                case RelationCategory.Validator: return "IsValidator = 1";
                default: throw ApplicationError.Create("Unexpected relation category: {0}", value.GetName());
            }
        }

        private static string CreateWhereForFilter(CmdsPersonFilter filter, string searchText)
        {
            var where = new StringBuilder();

            where.Append("1 = 1");

            if (filter.AccessGrantedToCmds != null)
            {
                if (filter.AccessGrantedToCmds.Value)
                    where.Append(" AND cmdsPerson.AccessGrantedToCmds = 1");
                else
                    where.Append(" AND cmdsPerson.AccessGrantedToCmds = 0");
            }

            if (filter.EnableIsArchived && filter.IsArchived.HasValue)
                where.Append($" AND cmdsPerson.UtcArchived {(filter.IsArchived.Value ? "IS NOT NULL" : "IS NULL")}");

            if (searchText.IsNotEmpty() || filter.Name.IsNotEmpty())
            {
                if (filter.NameFilterType == "Similar")
                    where.Append(" AND (cmdsPerson.FullName LIKE @Name OR cmdsPerson.SoundexFirstName = @SoundexName OR cmdsPerson.SoundexLastName = @SoundexName)");
                else
                    where.Append(" AND (cmdsPerson.FirstName LIKE @Name OR cmdsPerson.LastName LIKE @Name OR cmdsPerson.FullName LIKE @Name)");
            }

            if (filter.FirstName.IsNotEmpty())
            {
                if (filter.NameFilterType == "Similar")
                    where.Append(" AND (cmdsPerson.FirstName LIKE @FirstName OR cmdsPerson.SoundexFirstName = @SoundexFirstName)");
                else
                    where.Append(" AND (cmdsPerson.FirstName = @FirstName)");
            }

            if (filter.LastName.IsNotEmpty())
            {
                if (filter.NameFilterType == "Similar")
                    where.Append(" AND (cmdsPerson.LastName LIKE @LastName OR cmdsPerson.SoundexLastName = @SoundexLastName)");
                else
                    where.Append(" AND (cmdsPerson.LastName = @LastName)");
            }

            if (filter.ExcludeUserIdentifier.HasValue)
                where.Append(" AND cmdsPerson.UserIdentifier <> @ExcludeUserIdentifier");

            if (filter.ExcludeUserIdentifier.HasValue && filter.RelationCategory.HasValue)
            {
                where.AppendFormat(@"
                    AND cmdsPerson.UserIdentifier NOT IN (
                          SELECT FromUserIdentifier
                            FROM identities.UserConnection WITH (NOLOCK)
                            WHERE ToUserIdentifier = @ExcludeUserIdentifier
                              AND {0}
                        )
                    ", GetRelationCategoryFilter(filter.RelationCategory.Value));
            }

            var roles = new List<string>();

            if (filter.RelationCategory.HasValue)
            {
                switch (filter.RelationCategory)
                {
                    case RelationCategory.Manager:
                        roles.Add(CmdsRole.Managers);
                        break;
                    case RelationCategory.Supervisor:
                        roles.Add(CmdsRole.Supervisors);
                        break;
                    case RelationCategory.Validator:
                        roles.Add(CmdsRole.Validators);
                        break;
                }
            }

            if (filter.JobDivision.IsNotEmpty())
                where.Append(" AND P.JobDivision = @JobDivision");

            if (filter.KeyeraRoles.IsNotEmpty())
                roles.AddRange(filter.KeyeraRoles);

            if (roles.Count > 0)
            {
                where.AppendFormat(@"
                    AND cmdsPerson.UserIdentifier IN (
                          select UserIdentifier
                          from   contacts.Membership AS m  WITH (NOLOCK) inner join contacts.QGroup as g  WITH (NOLOCK) on g.GroupIdentifier = m.GroupIdentifier
                          where  g.GroupName in ({0})
                        )
                    ", CsvConverter.ConvertListToCsvText(roles, true));
            }

            if (filter.ParentUserIdentifier.HasValue)
            {
                var relationWithParent = string.Empty;

                if (filter.RelationWithParent.IsNotEmpty())
                {
                    relationWithParent += "AND (1=0";

                    foreach (var relationCategory in filter.RelationWithParent.Distinct())
                        relationWithParent += " OR " + GetRelationCategoryFilter(relationCategory);

                    relationWithParent += ")";
                }

                where.AppendFormat(@"
                    AND (
                         cmdsPerson.UserIdentifier = @ParentID
                      OR cmdsPerson.UserIdentifier IN (
                                SELECT ToUserIdentifier
                                  FROM identities.UserConnection WITH (NOLOCK)
                                  WHERE FromUserIdentifier = @ParentID
                                    {0}
                            )
                    )
                    ", relationWithParent);
            }

            if (filter.EmailWork.IsNotEmpty())
                where.Append(" AND cmdsPerson.Email LIKE @EmailWork");

            if (filter.EmployeeType.IsNotEmpty())
                where.Append(" AND cmdsPerson.UserIdentifier IN (SELECT UserIdentifier FROM contacts.QPerson WHERE EmployeeType LIKE @EmployeeType)");

            if (filter.PersonCode.IsNotEmpty())
                where.Append(" AND cmdsPerson.UserIdentifier IN (SELECT UserIdentifier FROM contacts.QPerson WHERE PersonCode LIKE @PersonCode)");

            if (filter.PersonType.IsNotEmpty())
                where.Append(" AND cmdsPerson.UserIdentifier IN (SELECT UserIdentifier FROM contacts.QPerson WHERE PersonType LIKE @PersonType)");

            string assignmentWhere;

            if (filter.RoleType.IsNotEmpty())
            {
                var membershipSubTypeList = CsvConverter.ConvertListToCsvText(filter.RoleType, true);
                assignmentWhere = string.Format(" AND m.MembershipType IN ({0})", membershipSubTypeList);
            }
            else
                assignmentWhere = null;

            if (filter.DepartmentsForParentId.HasValue && filter.OrganizationIdentifier.HasValue)
            {
                where.AppendFormat(@"
                    AND cmdsPerson.UserIdentifier IN (
                            SELECT m.UserIdentifier
                              FROM contacts.Membership AS m WITH (NOLOCK)
                              INNER JOIN contacts.Membership AS m_parent WITH (NOLOCK)
                                ON m_parent.GroupIdentifier = m.GroupIdentifier
                              INNER JOIN identities.Department dep WITH (NOLOCK)
                                ON dep.DepartmentIdentifier = m.GroupIdentifier
                              WHERE dep.OrganizationIdentifier = @OrganizationIdentifier
                                AND m_parent.UserIdentifier = @DepartmentsForParentID
                                {0}
                        )
                    ", assignmentWhere);
            }
            else if (filter.CompaniesForParentId.HasValue)
            {
                where.AppendFormat(@"
                    AND cmdsPerson.UserIdentifier IN (
                            SELECT m.UserIdentifier
                              FROM contacts.Membership AS m WITH (NOLOCK)
                              INNER JOIN contacts.Membership AS m_parent WITH (NOLOCK)
                                ON m_parent.GroupIdentifier = m.GroupIdentifier
                              INNER JOIN identities.Department AS dep WITH (NOLOCK)
                                ON dep.DepartmentIdentifier = m.GroupIdentifier
                              WHERE m_parent.UserIdentifier = @CompaniesForParentID
                                {0}
                        )
                    ", assignmentWhere);
            }
            else
            {
                if (filter.DepartmentIdentifier.HasValue)
                {
                    where.AppendFormat(@"
                        AND cmdsPerson.UserIdentifier IN (
                            SELECT m.UserIdentifier
                              FROM contacts.Membership m WITH (NOLOCK)
                              WHERE m.GroupIdentifier = @DepartmentIdentifier
                                {0}
                          )
                    ", assignmentWhere);
                }
                else if (filter.Departments.IsNotEmpty())
                {
                    where.AppendFormat(@"
                        AND cmdsPerson.UserIdentifier IN (
                            SELECT m.UserIdentifier
                              FROM contacts.Membership AS m WITH (NOLOCK)
                              WHERE m.GroupIdentifier IN ({0})
                                {1}
                          )
                    ", CsvConverter.ConvertListToCsvText(filter.Departments, true), assignmentWhere);
                }
                else if (filter.OrganizationIdentifier.HasValue)
                {
                    where.AppendFormat(@"
                        AND cmdsPerson.UserIdentifier IN (
                                SELECT m.UserIdentifier
                                  FROM contacts.Membership AS m WITH (NOLOCK)
                                  INNER JOIN identities.Department AS dep WITH (NOLOCK)
                                    ON dep.DepartmentIdentifier = m.GroupIdentifier
                                  WHERE dep.OrganizationIdentifier = @OrganizationIdentifier
                                    {0}
                                )
                    ", assignmentWhere);

                    if (filter.IsApproved.HasValue)
                    {
                        if (filter.IsApproved.Value)
                            where.Append(" AND cmdsPerson.UserIdentifier IN (SELECT UserIdentifier FROM contacts.QPerson WITH (NOLOCK)  WHERE OrganizationIdentifier = @OrganizationIdentifier AND UserAccessGranted IS NOT NULL)");
                        else
                            where.Append(" AND cmdsPerson.UserIdentifier NOT IN (SELECT UserIdentifier FROM contacts.QPerson WITH (NOLOCK)  WHERE OrganizationIdentifier = @OrganizationIdentifier AND UserAccessGranted IS NOT NULL)");
                    }
                }
            }

            if (filter.CompetencyStandardIdentifier.HasValue)
            {
                where.Append(@"
                    AND cmdsPerson.UserIdentifier IN (
                        SELECT UserIdentifier
                          FROM custom_cmds.UserCompetency WITH (NOLOCK)
                          WHERE CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                      )
                    ");
            }

            if (filter.AchievementIdentifier.HasValue)
            {
                where.Append(@"
                    AND cmdsPerson.UserIdentifier IN (
                        SELECT UserIdentifier
                          FROM custom_cmds.VCmdsCredential WITH (NOLOCK)
                          WHERE AchievementIdentifier = @AchievementIdentifier
                      )
                    ");
            }

            if (filter.ProfileStandardIdentifier.HasValue)
            {
                where.Append(@"
                    AND cmdsPerson.UserIdentifier IN (
                        SELECT UserIdentifier
                          FROM custom_cmds.UserProfile WITH (NOLOCK)
                          WHERE ProfileStandardIdentifier = @ProfileStandardIdentifier
                      )
                    ");
            }

            switch (filter.EmailStatus)
            {
                case "No Email":
                    where.Append(" AND cmdsPerson.Email IS NULL");
                    break;
                case "Valid Email":
                    where.AppendFormat(" AND cmdsPerson.Email LIKE '{0}'", Pattern.ValidEmailLike);
                    break;
                case "Invalid Email":
                    where.AppendFormat(" AND cmdsPerson.Email IS NOT NULL AND cmdsPerson.Email NOT LIKE '{0}'", Pattern.ValidEmailLike);
                    break;
            }

            if (filter.EmailEnabled.HasValue)
                where.Append(" AND P.EmailEnabled = @EmailEnabled");

            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForFilter(CmdsPersonFilter filter, string searchText, Guid? organization)
        {
            var phonetics = new Phonetics();
            var parameters = new List<SqlParameter>();

            if (organization.HasValue)
                parameters.Add(new SqlParameter("PersonOrganizationIdentifier", organization));

            if (searchText.IsNotEmpty())
            {
                parameters.Add(new SqlParameter("Name", string.Format("{0}%", searchText)));
                parameters.Add(new SqlParameter("SoundexName", phonetics.Soundex(searchText, 4, 0)));
            }

            else if (filter.Name.IsNotEmpty())
            {
                parameters.Add(new SqlParameter("Name", filter.NameFilterType == "Similar" ? $"%{filter.Name}%" : $"{filter.Name}%"));
                parameters.Add(new SqlParameter("SoundexName", phonetics.Soundex(filter.Name, 4, 0)));
            }

            if (filter.FirstName.IsNotEmpty())
            {
                parameters.Add(new SqlParameter("FirstName", filter.NameFilterType == "Similar" ? $"%{filter.FirstName}%" : filter.FirstName));
                parameters.Add(new SqlParameter("SoundexFirstName", phonetics.Soundex(filter.FirstName, 4, 0)));
            }

            if (filter.LastName.IsNotEmpty())
            {
                parameters.Add(new SqlParameter("LastName", filter.NameFilterType == "Similar" ? $"%{filter.LastName}%" : filter.LastName));
                parameters.Add(new SqlParameter("SoundexLastName", phonetics.Soundex(filter.LastName, 4, 0)));
            }

            if (filter.ExcludeUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ExcludeUserIdentifier", filter.ExcludeUserIdentifier));

            if (filter.RelationCategory.HasValue)
            {
                var category = filter.RelationCategory.GetName();

                parameters.Add(new SqlParameter("Category", category));
            }

            if (filter.ParentUserIdentifier.HasValue)
                parameters.Add(new SqlParameter("ParentID", filter.ParentUserIdentifier));

            if (filter.EmailWork.IsNotEmpty())
                parameters.Add(new SqlParameter("EmailWork", $"%{filter.EmailWork}%"));

            if (filter.OrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            if (filter.DepartmentIdentifier.HasValue)
                parameters.Add(new SqlParameter("DepartmentIdentifier", filter.DepartmentIdentifier));

            if (filter.CompetencyStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("CompetencyStandardIdentifier", filter.CompetencyStandardIdentifier));

            if (filter.AchievementIdentifier.HasValue)
                parameters.Add(new SqlParameter("AchievementIdentifier", filter.AchievementIdentifier));

            if (filter.ProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileStandardIdentifier", filter.ProfileStandardIdentifier));

            if (filter.DepartmentsForParentId.HasValue)
                parameters.Add(new SqlParameter("DepartmentsForParentID", filter.DepartmentsForParentId));

            if (filter.CompaniesForParentId.HasValue)
                parameters.Add(new SqlParameter("CompaniesForParentID", filter.CompaniesForParentId));

            if (filter.MailingListId.HasValue)
                parameters.Add(new SqlParameter("MailingListID", filter.MailingListId));

            if (filter.EmailEnabled.HasValue)
                parameters.Add(new SqlParameter("EmailEnabled", filter.EmailEnabled.Value));

            if (filter.JobDivision.IsNotEmpty())
                parameters.Add(new SqlParameter("JobDivision", filter.JobDivision));

            if (filter.EmployeeType.IsNotEmpty())
                parameters.Add(new SqlParameter("EmployeeType", $"%{filter.EmployeeType}%"));

            if (filter.PersonCode.IsNotEmpty())
                parameters.Add(new SqlParameter("PersonCode", $"%{filter.PersonCode}%"));

            if (filter.PersonType.IsNotEmpty())
                parameters.Add(new SqlParameter("PersonType", $"%{filter.PersonType}%"));

            return parameters;
        }

        #endregion

        #region Select by RoleFilter

        public class CmdsRoleEntity
        {
            public Guid GroupIdentifier { get; set; }
            public string GroupName { get; set; }
        }

        public static List<CmdsRoleEntity> SelectRoles()
        {
            const string query = @"
SELECT
    GroupIdentifier
  , GroupName
FROM
    contacts.QGroup
WHERE
    GroupType = 'Role'
    AND
        (
            GroupName LIKE 'CMDS %'
            OR GroupName LIKE 'Skills Passport %'
        )
ORDER BY
    GroupName;
";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<CmdsRoleEntity>(query).ToList();
        }

        public static List<CmdsRoleEntity> SelectRoles(Guid? user)
        {
            const string query = @"
SELECT
    r.GroupIdentifier
   ,r.GroupName
FROM
    custom_cmds.PermissionList r
{0}
ORDER BY
    r.GroupName
";

            var where = CreateWhereForRoleFilter(user);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<CmdsRoleEntity>(curQuery, GetParametersForRoleFilter(user).ToArray()).ToList();
        }

        public static DataTable SelectForRoleSelector()
        {
            const string query = @"
SELECT
    GroupIdentifier AS [Value]
   ,GroupName AS [Text]
FROM
    contacts.QGroup
    INNER JOIN accounts.QOrganization ON QOrganization.OrganizationIdentifier = QGroup.OrganizationIdentifier
WHERE
    QOrganization.OrganizationCode IN ( 'cmds', 'keyera' )
    AND GroupType = 'Role'
ORDER BY
    GroupName;
";

            return DatabaseHelper.CreateDataTable(query);
        }

        private static List<SqlParameter> GetParametersForRoleFilter(Guid? user)
        {
            var parameters = new List<SqlParameter>();

            if (user != null)
                parameters.Add(new SqlParameter("UserID", user));

            return parameters;
        }

        private static String CreateWhereForRoleFilter(Guid? user)
        {
            var where = new StringBuilder();

            where.Append("WHERE GroupName IS NOT NULL");

            if (user != null)
            {
                where.Append(@"
AND R.GroupIdentifier IN (
    select m.GroupIdentifier
    from contacts.Membership as m
    where m.UserIdentifier = @UserID
)");
            }

            return where.ToString();
        }

        #endregion
    }
}