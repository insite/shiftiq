using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TSenderStore
    {
        public static void Update(TSender entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Insert(TSender entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TSenders.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TSenders.FirstOrDefault(x => x.SenderIdentifier == id);
                if (entity == null)
                    return;
                db.TSenders.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
