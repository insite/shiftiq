using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ProfileCertificationRepository
    {
        public static ProfileCertification Select(Guid userKey, Guid profileStandardIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.ProfileCertifications.FirstOrDefault(x => x.UserIdentifier == userKey && x.ProfileStandardIdentifier == profileStandardIdentifier);
        }

        public static DataTable SelectByFilter(EmployeeCertificateFilter filter)
        {
            const String query = @"
WITH OrderedEmployeeCertificates AS
(
    SELECT
        cert.ProfileStandardIdentifier AS ProfileIdentifier,
        cert.DateRequested,
        cert.DateGranted,
        cert.DateSubmitted,
        cert.UserIdentifier AS LearnerIdentifier,
        cert.AuthorityName AS AuthorityName,
        persons.FullName as LearnerName,
        profiles.ProfileNumber,
        profiles.ProfileTitle,
        [cert].AuthorityName AS InstitutionName,
        ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumber
    FROM
        custom_cmds.ProfileCertification [cert]
        INNER JOIN identities.[User] persons ON persons.UserIdentifier = [cert].UserIdentifier
        INNER JOIN custom_cmds.[Profile] profiles ON profiles.ProfileStandardIdentifier = [cert].ProfileStandardIdentifier
    {0}
)
SELECT * FROM OrderedEmployeeCertificates
    WHERE RowNumber BETWEEN @StartRow AND @EndRow
    ORDER BY RowNumber
                ";

            String where = CreateWhereForFilter(filter);

            var sortExpression = "DateRequested DESC";

            String withSortExpression = sortExpression
                .Replace("InstitutionName", "[cert].AuthorityName")
                .Replace("FullName", "persons.FullName")
                ;

            var curQuery = string.Format(query, where, withSortExpression);

            var (startRow, endRow) = filter.Paging != null ? filter.Paging.ToStartEnd() : (0, int.MaxValue);

            var parameters = GetParametersForFilter(filter);
            parameters.Add(new SqlParameter("StartRow", startRow));
            parameters.Add(new SqlParameter("EndRow", endRow));

            return DatabaseHelper.CreateDataTable(curQuery, 5 * 60, parameters.ToArray());
        }

        public static int CountByFilter(EmployeeCertificateFilter filter)
        {
            const string query = @"
                  SELECT COUNT(*)
                    FROM custom_cmds.ProfileCertification [cert]
                    INNER JOIN identities.[User] persons
                      ON persons.UserIdentifier = [cert].UserIdentifier
                    INNER JOIN custom_cmds.[Profile] profiles
                      ON profiles.ProfileStandardIdentifier = [cert].ProfileStandardIdentifier
                    {0}
                ";

            var where = CreateWhereForFilter(filter);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersForFilter(filter).ToArray()).FirstOrDefault();
        }

        private static String CreateWhereForFilter(EmployeeCertificateFilter filter)
        {
            StringBuilder where = new StringBuilder();
            where.Append("WHERE 1 = 1");

            if (filter.UserIdentifier.HasValue)
                where.Append(" AND [cert].UserIdentifier = @UserIdentifier");

            if (!String.IsNullOrEmpty(filter.EmployeeName))
                where.Append(" AND persons.FullName LIKE @EmployeeName");

            if (!String.IsNullOrEmpty(filter.ProfileTitle))
                where.Append(" AND profiles.ProfileTitle LIKE @ProfileTitle");

            return where.ToString();
        }

        private static List<SqlParameter> GetParametersForFilter(EmployeeCertificateFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (filter.UserIdentifier.HasValue)
                parameters.Add(new SqlParameter("UserIdentifier", filter.UserIdentifier));

            if (!String.IsNullOrEmpty(filter.EmployeeName))
                parameters.Add(new SqlParameter("EmployeeName", string.Format("%{0}%", filter.EmployeeName)));

            if (!String.IsNullOrEmpty(filter.ProfileTitle))
                parameters.Add(new SqlParameter("ProfileTitle", string.Format("%{0}%", filter.ProfileTitle)));

            return parameters;
        }
    }
}
