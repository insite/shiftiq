using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class LtiLinkStore
    {
        public static void Insert(TLtiLink entity)
        {
            using (var db = new InternalDbContext())
            {
                db.LtiLinks.Add(entity);
                db.SaveChanges();
            }
        }

        public static TLtiLink Insert(Action<TLtiLink> action)
        {
            var entity = new TLtiLink();

            action(entity);

            Insert(entity);

            return entity;
        }

        public static void Update(TLtiLink entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.LtiLinks.FirstOrDefault(x => x.LinkIdentifier == id);
                db.LtiLinks.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
