using System.Linq;

namespace InSite.Persistence
{
    public class UpgradeSearch
    {
        public static Upgrade Select(string name)
        {
            using (var db = new InternalDbContext())
            {
                return db.Upgrades.FirstOrDefault(x => x.ScriptName == name);
            }
        }

        public static bool Exists(string name)
        {
            using (var db = new InternalDbContext())
            {
                return db.Upgrades.Any(x => x.ScriptName == name);
            }
        }

        public static string GetData(string name)
        {
            using (var db = new InternalDbContext())
            {
                return db.Upgrades.FirstOrDefault(x => x.ScriptName == name)?.ScriptData;
            }
        }
    }
}
