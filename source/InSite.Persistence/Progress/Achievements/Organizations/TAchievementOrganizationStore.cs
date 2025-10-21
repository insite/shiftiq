using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TAchievementOrganizationStore
    {
        public static void InsertOrganizationAchievement(Guid organization, Guid achievemenet)
        {
            using (var db = new InternalDbContext())
            {
                var ta = db.TAchievementOrganizations
                    .FirstOrDefault(x => x.OrganizationIdentifier == organization && x.AchievementIdentifier == achievemenet);
                if (ta == null)
                {
                    ta = new TAchievementOrganization
                    {
                        OrganizationIdentifier = organization,
                        AchievementIdentifier = achievemenet
                    };
                    db.TAchievementOrganizations.Add(ta);
                    db.SaveChanges();
                }
            }
        }

        public static void Insert(IEnumerable<TAchievementOrganization> list)
        {
            using (var db = new InternalDbContext())
            {
                db.TAchievementOrganizations.AddRange(list);
                db.SaveChanges();
            }
        }

        public static void Delete(IEnumerable<TAchievementOrganization> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in list)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }
    }
}
