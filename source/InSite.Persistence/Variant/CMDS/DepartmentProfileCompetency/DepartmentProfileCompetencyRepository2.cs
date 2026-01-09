using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class DepartmentProfileCompetencyRepository2
    {
        #region Paged select

        public static DataTable SelectCompetencies(Guid department, Paging paging)
        {
            const String query = @"
WITH OrderedCompanyCompetencies AS
(
    SELECT
        c.CompetencyStandardIdentifier
       ,c.Number
       ,c.NumberOld
       ,c.Summary
       ,p.ProfileStandardIdentifier
       ,p.ProfileNumber
       ,p.ProfileTitle
       ,cs.Criticality
       ,CASE WHEN cs.ValidForCount IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsTimeSensitive
       ,cs.ValidForCount
       ,cs.ValidForUnit
       ,ROW_NUMBER() OVER(ORDER BY c.Number, c.NumberOld, p.ProfileNumber, p.ProfileTitle) AS RowNumber
    FROM
        custom_cmds.Competency c
        INNER JOIN custom_cmds.DepartmentProfileCompetency cs
            ON cs.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
        INNER JOIN custom_cmds.ProfileCompetency pc
            ON pc.CompetencyStandardIdentifier = cs.CompetencyStandardIdentifier
               AND pc.ProfileStandardIdentifier = cs.ProfileStandardIdentifier
        INNER JOIN custom_cmds.[Profile] p
            ON p.ProfileStandardIdentifier = cs.ProfileStandardIdentifier
    WHERE
        cs.DepartmentIdentifier = @DepartmentIdentifier
        AND c.IsDeleted = 0
)
SELECT * FROM OrderedCompanyCompetencies
WHERE RowNumber BETWEEN @StartRow AND @EndRow
ORDER BY Number, NumberOld, ProfileNumber, ProfileTitle;";

            var (startRow, endRow) = paging.ToStartEnd();

            return DatabaseHelper.CreateDataTable(query,
                new SqlParameter("DepartmentIdentifier", department),
                new SqlParameter("StartRow", startRow),
                new SqlParameter("EndRow", endRow)
                );
        }

        public static int SelectCompetencyCount(Guid department)
        {
            const String query = @"
SELECT COUNT(*)
FROM custom_cmds.DepartmentProfileCompetency
WHERE DepartmentIdentifier = @DepartmentIdentifier;";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, new SqlParameter("DepartmentIdentifier", department)).FirstOrDefault();
        }

        #endregion

        #region SELECT

        public static DataTable SelectCompetenciesByOrganizationIdentifier(Guid organizationId, Guid competencyId, int? pageIndex, int? pageSize)
        {
            String query = @"
                SELECT
                    departments.DepartmentIdentifier,
                    departments.DepartmentName,
                    CASE WHEN settings.ValidForCount IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsTimeSensitive,
                    settings.ValidForCount,
                    settings.ValidForUnit,
                    settings.PriorityName

                  FROM (
                      SELECT DISTINCT
                          d.DepartmentIdentifier,
                          d.DepartmentName
                        FROM custom_cmds.DepartmentProfileCompetency cs
                        INNER JOIN identities.Department as d
                          ON d.DepartmentIdentifier = cs.DepartmentIdentifier
                        WHERE d.OrganizationIdentifier = @OrganizationIdentifier
                          AND cs.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                    ) AS departments

                  CROSS APPLY (
                      SELECT TOP 1
                          cs.ValidForCount,
                          cs.ValidForUnit,
                          ISNULL(cs.Criticality, 'Non-Critical') AS PriorityName
                        FROM custom_cmds.DepartmentProfileCompetency cs
                        WHERE cs.DepartmentIdentifier = departments.DepartmentIdentifier
                          AND cs.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                        ORDER BY PriorityName
                    ) AS settings
                ";

            if (pageIndex.HasValue)
            {
                query += @"
ORDER BY DepartmentName
OFFSET (@PageIndex * @PageSize)
ROWS FETCH NEXT @PageSize ROWS ONLY
";
            }

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("OrganizationIdentifier", organizationId));
            parameters.Add(new SqlParameter("CompetencyStandardIdentifier", competencyId));

            if (pageIndex.HasValue)
            {
                parameters.Add(new SqlParameter("PageIndex", pageIndex));
                parameters.Add(new SqlParameter("PageSize", pageSize));
            }

            return DatabaseHelper.CreateDataTable(query, parameters.ToArray());
        }

        public static DataTable SelectCompetenciesByProfileId(Guid[] departments, Guid profileStandardIdentifier, Guid organizationId,
                                                       Int32 competencyStartRow, Int32 competencyEndRow)
        {
            const String query = @"
                WITH OrderedCompetencies AS
                (
                  SELECT
                      c.*,
                      pc.ProfileStandardIdentifier,
                      ROW_NUMBER() OVER(ORDER BY c.Number, c.Summary) AS RowNumber
                    FROM custom_cmds.Competency c
                    INNER JOIN custom_cmds.ProfileCompetency pc
                      ON pc.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
                    WHERE pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
                      AND c.IsDeleted = 0
                )
                SELECT
                    cs.*
                  FROM OrderedCompetencies c
                  INNER JOIN custom_cmds.DepartmentProfileCompetency cs
                    ON cs.CompetencyStandardIdentifier = c.CompetencyStandardIdentifier
                      AND cs.ProfileStandardIdentifier = c.ProfileStandardIdentifier
                  INNER JOIN identities.Department as dep
                    ON dep.DepartmentIdentifier = cs.DepartmentIdentifier
                  WHERE dep.OrganizationIdentifier = @OrganizationIdentifier
                    AND RowNumber BETWEEN @CompetencyStartRow AND @CompetencyEndRow
                    AND cs.DepartmentIdentifier IN ({0})
                ";

            var departmentList = CsvConverter.ConvertListToCsvText(departments, true);
            var curQuery = string.Format(query, departmentList);

            return DatabaseHelper.CreateDataTable(curQuery,
                new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier),
                new SqlParameter("OrganizationIdentifier", organizationId),
                new SqlParameter("CompetencyStartRow", competencyStartRow),
                new SqlParameter("CompetencyEndRow", competencyEndRow)
                );
        }

        #endregion

        #region SelectCompetencyLevels

        public static int SelectCompetencyLevels(Guid department)
        {
            const string query = @"
SELECT
    COUNT(DISTINCT cs.CompetencyStandardIdentifier) AS CompetencyCount
FROM custom_cmds.DepartmentProfileCompetency cs
WHERE cs.DepartmentIdentifier = @DepartmentIdentifier
                ";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(query, new SqlParameter("DepartmentIdentifier", department)).FirstOrDefault();
        }

        #endregion

        #region UPDATE \ INSERT \ DELETE

        public static void InsertProfileCompetencies(Guid department, Guid profileStandardIdentifier)
        {
            const string query = "EXEC custom_cmds.InsertDepartmentProfileCompetencies @DepartmentIdentifier, @ProfileStandardIdentifier";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query, new SqlParameter[] { new SqlParameter("@DepartmentIdentifier", department), new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier) });
            }
        }

        public static void DeleteUnusedByProfileId(Guid profileStandardIdentifier)
        {
            const string query = "EXEC custom_cmds.DeleteUnusedDepartmentProfileCompetencies1 @ProfileStandardIdentifier";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query, new SqlParameter("@ProfileStandardIdentifier", profileStandardIdentifier));
            }
        }

        public static void DeleteUnusedByDepartmentIdentifier(Guid department)
        {
            const string query = "EXEC custom_cmds.DeleteUnusedDepartmentProfileCompetencies2 @DepartmentIdentifier";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query, new SqlParameter("@DepartmentIdentifier", department));
            }
        }

        #endregion
    }
}
