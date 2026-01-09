using System;
using System.Data.Entity;

namespace InSite.Persistence
{
    public class UpgradeStore
    {
        public static void Save(string name, DateTimeOffset when, string data = null)
        {
            if (UpgradeSearch.Exists(name))
                return;

            using (var db = new InternalDbContext())
            {
                var upgrade = new Upgrade
                {
                    UpgradeIdentifier = Guid.NewGuid(),
                    ScriptName = name,
                    UtcUpgraded = when,
                    ScriptData = data
                };
                db.Upgrades.Add(upgrade);
                db.SaveChanges();
            }
        }

        public static void Update(Upgrade item)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
