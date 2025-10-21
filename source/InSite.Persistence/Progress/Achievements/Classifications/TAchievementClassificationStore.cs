using System.Collections.Generic;
using System.Data.Entity;

namespace InSite.Persistence
{
    public static class TAchievementClassificationStore
    {
        public static void Insert(TAchievementCategory entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TAchievementCategories.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Insert(IEnumerable<TAchievementCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                db.TAchievementCategories.AddRange(list);
                db.SaveChanges();
            }
        }

        public static void Insert(VAchievementClassification item)
        {
            Insert(new TAchievementCategory { AchievementIdentifier = item.AchievementIdentifier, ItemIdentifier = item.CategoryIdentifier });
        }

        public static void Insert(IEnumerable<VAchievementClassification> list)
        {
            var inserts = new List<TAchievementCategory>();
            foreach (var entity in list)
                inserts.Add(new TAchievementCategory { AchievementIdentifier = entity.AchievementIdentifier, ItemIdentifier = entity.CategoryIdentifier });
            Insert(inserts);
        }

        public static void Delete(IEnumerable<TAchievementCategory> list)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in list)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }

        public static void Delete(IEnumerable<VAchievementClassification> list)
        {
            var deletes = new List<TAchievementCategory>();
            foreach (var entity in list)
                deletes.Add(new TAchievementCategory { AchievementIdentifier = entity.AchievementIdentifier, ItemIdentifier = entity.CategoryIdentifier });
            Delete(deletes);
        }
    }
}
