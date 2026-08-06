using System.Linq;

namespace InSite.Persistence
{
    public static class TToolkitUsageStore
    {
        public static int Calculate()
        {
            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<int>("exec reports.CalcToolkitUsage").First();
            }
        }
    }
}
