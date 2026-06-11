using System;
using System.Data.Entity;
using System.Linq;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class TAchievementCategoryStore
    {
        public static void Insert(VAchievementCategory entity)
        {
            using (var db = new InternalDbContext())
            {
                var collection = db.TCollections.FirstOrDefault(x => x.CollectionName == CollectionName.Learning_Catalogs_Category_Name);

                if (collection == null)
                    return;

                var item = new TCollectionItem
                {
                    CollectionIdentifier = collection.CollectionIdentifier,
                    ItemIdentifier = entity.CategoryIdentifier,
                    ItemName = entity.CategoryName,
                    ItemDescription = entity.CategoryDescription,
                    OrganizationIdentifier = entity.OrganizationIdentifier
                };

                db.TCollectionItems.Add(item);

                db.SaveChanges();
            }
        }

        public static void Update(VAchievementCategory entity)
        {
            using (var db = new InternalDbContext())
            {
                var item = db.TCollectionItems.FirstOrDefault(x => x.ItemIdentifier == entity.CategoryIdentifier);

                item.ItemFolder = entity.AchievementLabel;
                item.ItemName = entity.CategoryName;
                item.ItemDescription = entity.CategoryDescription;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid category)
        {
            using (var db = new InternalDbContext())
            {
                var item = db.TCollectionItems.FirstOrDefault(x => x.ItemIdentifier == category);

                if (item == null)
                    return;

                var list = db.TAchievementCategories.Where(x => x.ItemIdentifier == category);

                db.TAchievementCategories.RemoveRange(list);

                db.TCollectionItems.Remove(item);

                db.SaveChanges();
            }
        }
    }
}
