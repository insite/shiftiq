using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TUserStatusStore
    {
        private static void ExecuteSlowCommand(Database database, string sql, params object[] parameters)
        {
            const int fiveMinutes = 60 * 5;

            database.CommandTimeout = fiveMinutes;

            database.ExecuteSqlCommand(sql, parameters);
        }

        public static void CreateSnapshot(Guid? organization)
        {
            var p1 = new SqlParameter("@OrganizationIdentifier", organization);
            var p2 = new SqlParameter("@DepartmentIdentifier", SqlDbType.UniqueIdentifier);
            var p3 = new SqlParameter("@UserIdentifier", SqlDbType.UniqueIdentifier);

            p2.Value = p3.Value = DBNull.Value;

            using (var db = new InternalDbContext())
            {
                ExecuteSlowCommand(db.Database, "custom_cmds.RefreshTUserStatus @OrganizationIdentifier, @DepartmentIdentifier, @UserIdentifier", p1, p2, p3);
            }
        }

        public static void Delete(Guid organization, DateTimeOffset asAt)
        {
            var p1 = new SqlParameter("@OrganizationIdentifier", organization);
            var p2 = new SqlParameter("@AsAt", asAt);

            using (var db = new InternalDbContext())
            {
                ExecuteSlowCommand(db.Database, "DELETE custom_cmds.TUserStatus WHERE OrganizationIdentifier = @OrganizationIdentifier AND AsAt = @AsAt;", p1, p2);
            }
        }

        public static void Refresh()
        {
            using (var db = new InternalDbContext())
            {
                ExecuteSlowCommand(db.Database, "EXEC custom_cmds.RefreshQUserStatus");
            }
        }
    }
}