using System;
using System.Collections.Generic;
using System.Data.Entity;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCollectionItemStore
    {
        public static TCollectionItem Update(TCollectionItem entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return entity;
            }
        }

        public static void Update(IEnumerable<TCollectionItem> entities)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in entities)
                    db.Entry(entity).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public static TCollectionItem Insert(TCollectionItem entity)
        {
            using (var db = new InternalDbContext())
            {
                if (entity.ItemIdentifier == Guid.Empty)
                    entity.ItemIdentifier = UniqueIdentifier.Create();

                db.TCollectionItems.Add(entity);
                db.SaveChanges();
                return entity;
            }
        }

        public static void Delete(Guid itemId)
        {
            using (var db = new InternalDbContext())
            {
                var entity = new TCollectionItem { ItemIdentifier = itemId };

                db.TCollectionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static void Delete(IEnumerable<TCollectionItem> entities)
        {
            using (var db = new InternalDbContext())
            {
                foreach (var entity in entities)
                    db.Entry(entity).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }
    }
}