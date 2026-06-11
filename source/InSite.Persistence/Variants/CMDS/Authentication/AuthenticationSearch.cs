using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class AuthenticationSearch
    {
        public static DataTable SelectByFilter(AuthenticationFilter filter)
        {
            const string query = @"
    SessionCode
   ,SessionStarted
   ,UserBrowser
   ,UserHostAddress
   ,UserLanguage
   ,UserEmail
   ,UserAgent
   ,CASE WHEN SessionIsAuthenticated = 1 THEN 'Success' ELSE 'Failed' END AS EventName
   ,AuthenticationErrorType
FROM
    [security].TUserSession
{0}
ORDER BY
    {1}
";

            var sortExpression = "SessionStarted DESC, UserEmail";

            var where = CreateWhereByFilter(filter);
            var curQuery = string.Format(query, where, sortExpression);

            var parameters = GetParametersByFilter(filter);

            if (filter.Paging == null)
            {
                curQuery = "SELECT " + curQuery;
            }
            else
            {
                var (startRow, endRow) = filter.Paging.ToStartEnd();

                if (startRow == 1 && endRow == 0)
                {
                    curQuery = "SELECT TOP 0 " + curQuery;
                }
                else
                {
                    curQuery = "SELECT " + curQuery + @"OFFSET (@StartRow - 1) ROWS FETCH NEXT (@EndRow - @StartRow + 1) ROWS ONLY";

                    parameters.Add(new SqlParameter("StartRow", startRow));
                    parameters.Add(new SqlParameter("EndRow", endRow));
                }
            }

            return DatabaseHelper.CreateDataTable(curQuery, parameters.ToArray());
        }

        public static int CountByFilter(AuthenticationFilter filter)
        {
            const string query = "SELECT COUNT(*) FROM [security].TUserSession {0}";

            var where = CreateWhereByFilter(filter);
            var curQuery = string.Format(query, where);

            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<int>(curQuery, GetParametersByFilter(filter).ToArray()).FirstOrDefault();
        }

        private static List<SqlParameter> GetParametersByFilter(AuthenticationFilter filter)
        {
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(filter.UserEmail))
                parameters.Add(new SqlParameter("UserEmail", string.Format("%{0}%", filter.UserEmail)));

            if (filter.SinceDate.HasValue)
                parameters.Add(new SqlParameter("SinceDate", filter.SinceDate.Value.Date));

            if (filter.BeforeDate.HasValue)
                parameters.Add(new SqlParameter("BeforeDate", filter.BeforeDate.Value.Date.AddDays(1)));

            return parameters;
        }

        private static string CreateWhereByFilter(AuthenticationFilter filter)
        {
            var where = new StringBuilder();
            where.Append("WHERE UserEmail IN (SELECT Email FROM identities.[User] WHERE AccessGrantedToCmds = 1)");

            if (filter.UserEmail.HasValue())
                where.Append(" AND UserEmail LIKE @UserEmail");

            if (filter.SinceDate.HasValue)
                where.Append(" AND SessionStarted >= @SinceDate");

            if (filter.BeforeDate.HasValue)
                where.Append(" AND SessionStarted < @BeforeDate");

            if (filter.SessionIsAuthenticated.HasValue)
            {
                if (filter.SessionIsAuthenticated.Value)
                    where.Append(" AND SessionIsAuthenticated = 1");
                else
                    where.Append(" AND SessionIsAuthenticated = 0");
            }

            return where.ToString();
        }
    }
}