using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class VCmdsProfileOrganizationRepository
    {
        public static DataTable SelectUnusedProfiles(Guid organizationId)
        {
            const string query = @"
SELECT
    p.*
FROM
    custom_cmds.VCmdsProfileOrganization cp
    INNER JOIN custom_cmds.[Profile] p
        ON p.ProfileStandardIdentifier = cp.ProfileStandardIdentifier
WHERE
    cp.OrganizationIdentifier = @OrganizationIdentifier
    AND cp.ProfileStandardIdentifier NOT IN (
        SELECT
            dp.ProfileStandardIdentifier
        FROM
            custom_cmds.DepartmentProfile dp
            INNER JOIN identities.Department
                ON Department.DepartmentIdentifier = dp.DepartmentIdentifier
        WHERE
            Department.OrganizationIdentifier = @OrganizationIdentifier
    )
ORDER BY
    p.ProfileNumber
   ,p.ProfileTitle;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("OrganizationIdentifier", organizationId));
        }

        public static bool IsCompetencyInCompany(Guid organizationId, Guid competencyId)
        {
            const string query = @"
                SELECT 1
                  FROM custom_cmds.VCmdsProfileOrganization cp
                  INNER JOIN custom_cmds.ProfileCompetency pc
                    ON pc.ProfileStandardIdentifier = cp.ProfileStandardIdentifier
                  WHERE pc.CompetencyStandardIdentifier = @CompetencyStandardIdentifier
                    AND cp.OrganizationIdentifier = @OrganizationIdentifier
                ";

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int?>(query, new SqlParameter("OrganizationIdentifier", organizationId), new SqlParameter("CompetencyStandardIdentifier", competencyId)).FirstOrDefault() != null;
        }
    }
}
