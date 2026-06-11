using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ProfileCompetencyRepository
    {
        public static ProfileCompetency Select(Guid profileStandardIdentifier, Guid competencyId)
        {
            using (var db = new InternalDbContext())
                return db.ProfileCompetencies.FirstOrDefault(x => x.ProfileStandardIdentifier == profileStandardIdentifier && x.CompetencyStandardIdentifier == competencyId);
        }

        public static DataTable SelectSimilarities(Guid mainProfileStandardIdentifier, Guid profileStandardIdentifier)
        {
            const String query = @"
SELECT
    c.*
FROM
    (
        SELECT CompetencyStandardIdentifier
        FROM custom_cmds.ProfileCompetency
        WHERE ProfileStandardIdentifier = @ID1

        INTERSECT

        SELECT CompetencyStandardIdentifier
        FROM custom_cmds.ProfileCompetency
        WHERE ProfileStandardIdentifier = @ID2
    ) pc
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
WHERE
    c.IsDeleted = 0
ORDER BY
    c.Number;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ID1", mainProfileStandardIdentifier), new SqlParameter("ID2", profileStandardIdentifier));
        }

        public static DataTable SelectDifferences(Guid mainProfileStandardIdentifier, Guid profileStandardIdentifier)
        {
            const String query = @"
SELECT
    c.CompetencyStandardIdentifier
   ,c.*
FROM
    (
        SELECT CompetencyStandardIdentifier
        FROM custom_cmds.ProfileCompetency
        WHERE ProfileStandardIdentifier = @ID1

        EXCEPT

        SELECT CompetencyStandardIdentifier
        FROM custom_cmds.ProfileCompetency
        WHERE ProfileStandardIdentifier = @ID2
    ) pc
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
WHERE
    c.IsDeleted = 0
ORDER BY
    c.Number;";

            return DatabaseHelper.CreateDataTable(query, new SqlParameter("ID1", mainProfileStandardIdentifier), new SqlParameter("ID2", profileStandardIdentifier));
        }

        public static int SelectCount(Guid profileStandardIdentifier, bool includeDeleted)
        {
            const String query = @"
SELECT
    COUNT(*)
FROM
    custom_cmds.ProfileCompetency pc
    INNER JOIN custom_cmds.Competency c
        ON c.CompetencyStandardIdentifier = pc.CompetencyStandardIdentifier
WHERE
    pc.ProfileStandardIdentifier = @ProfileStandardIdentifier
    {0}";

            var where = includeDeleted ? "" : "AND c.IsDeleted = 0";
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, new SqlParameter("ProfileStandardIdentifier", profileStandardIdentifier)).FirstOrDefault();
        }

        public static DataTable SelectByFilter(ProfileCompetencyFilter filter, string sortExpression)
        {
            var where = CreateWhereForSelectSearchResults(filter);

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "ProfileNumber, ProfileTitle";

            var query = string.Format(@"
SELECT
    p.ProfileStandardIdentifier
   ,p.ProfileNumber
   ,p.ProfileTitle
   ,pc.CompetencyStandardIdentifier
FROM
    custom_cmds.ProfileCompetency pc
    INNER JOIN custom_cmds.[Profile] p
        ON p.ProfileStandardIdentifier = pc.ProfileStandardIdentifier
{0}
ORDER BY
    {1}", where, sortExpression);

            return DatabaseHelper.CreateDataTable(query, GetParametersForSelectSearchResults(filter).ToArray());
        }

        private static List<SqlParameter> GetParametersForSelectSearchResults(ProfileCompetencyFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (filter.ProfileStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("ProfileStandardIdentifier", filter.ProfileStandardIdentifier));

            if (filter.CompetencyStandardIdentifier.HasValue)
                parameters.Add(new SqlParameter("CompetencyStandardIdentifier", filter.CompetencyStandardIdentifier));

            if (filter.OrganizationIdentifier.HasValue)
                parameters.Add(new SqlParameter("OrganizationIdentifier", filter.OrganizationIdentifier));

            return parameters;
        }

        private static String CreateWhereForSelectSearchResults(ProfileCompetencyFilter filter)
        {
            StringBuilder where = new StringBuilder();

            where.Append("WHERE 1 = 1");

            if (filter.ProfileStandardIdentifier.HasValue)
                where.Append(" AND pc.ProfileStandardIdentifier = @ProfileStandardIdentifier");

            if (filter.CompetencyStandardIdentifier.HasValue)
                where.Append(" AND pc.CompetencyStandardIdentifier = @CompetencyStandardIdentifier");

            if (filter.OrganizationIdentifier.HasValue)
            {
                where.Append(@"
AND pc.ProfileStandardIdentifier IN (
    SELECT ProfileStandardIdentifier
        FROM custom_cmds.VCmdsProfileOrganization
        WHERE OrganizationIdentifier = @OrganizationIdentifier
)");
            }

            return where.ToString();
        }
    }
}
