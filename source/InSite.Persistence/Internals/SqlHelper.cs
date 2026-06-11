using System;
using System.Linq;

using InSite.Application;

namespace InSite.Persistence
{
    public static class SqlHelper
    {
        public static void SetIdentification(Func<ServiceIdentity> identification)
        {
            InternalDbContext.SetIdentification(identification);
        }

        public static int ExecuteWithTimeout(string sql, int commandTimeout, params object[] parameters)
        {
            using (var db = new InternalDbContext())
            {
                db.Database.CommandTimeout = commandTimeout;
                return db.Database.ExecuteSqlCommand(sql, parameters);
            }
        }

        public static int Execute(string sql, params object[] parameters)
        {
            using (var db = new InternalDbContext())
                return db.Database.ExecuteSqlCommand(sql, parameters);
        }

        public static T[] Query<T>(string sql, params object[] parameters)
        {
            using (var db = new InternalDbContext())
                return db.Database.SqlQuery<T>(sql, parameters).ToArray();
        }
    }
}
