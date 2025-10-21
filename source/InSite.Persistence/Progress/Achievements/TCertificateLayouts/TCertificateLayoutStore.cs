using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TCertificateLayoutStore
    {
        public static void Save(TCertificateLayout entity)
        {
            if (TCertificateLayoutSearch.Exists(entity.CertificateLayoutIdentifier))
                Update(entity);
            else
                Insert(entity);
        }

        public static void Update(TCertificateLayout entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Insert(TCertificateLayout entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TCertificateLayouts.Add(entity);
                db.SaveChanges();
            }
        }

        public static bool Delete(string code)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TCertificateLayouts.FirstOrDefault(x => x.CertificateLayoutCode == code);
                if (entity == null)
                    return false;

                db.TCertificateLayouts.Remove(entity);
                db.SaveChanges();
            }

            return true;
        }

        public static bool Delete(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                var layout = db.TCertificateLayouts.FirstOrDefault(x => x.CertificateLayoutIdentifier == id);
                if (layout == null)
                    return false;

                db.TCertificateLayouts.Remove(layout);
                db.SaveChanges();
            }

            return true;
        }
    }
}
