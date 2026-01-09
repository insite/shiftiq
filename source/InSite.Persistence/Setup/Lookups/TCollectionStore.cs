using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TCollectionStore
    {
        public static TCollection Update(TCollection entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return entity;
            }
        }

        public static TCollection Insert(TCollection entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TCollections.Add(entity);
                db.SaveChanges();
            }

            return entity;
        }

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TCollections.FirstOrDefault(x => x.CollectionIdentifier == id);
                if (entity == null)
                    return;

                var items = db.TCollectionItems.Where(x => x.CollectionIdentifier == id).ToArray();

                db.TCollectionItems.RemoveRange(items);
                db.TCollections.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
