using System;
using System.Data.Entity;
using System.Linq;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCollegeCertificateStore
    {
        public static void Delete(Guid certificate)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TCollegeCertificates.SingleOrDefault(x => x.CertificateIdentifier == certificate);
                if (entity == null)
                    return;

                db.TCollegeCertificates.Remove(entity);
                db.SaveChanges();
            }
        }

        public static void Insert(TCollegeCertificate entity)
        {
            using (var db = new InternalDbContext())
            {
                if (entity.CertificateIdentifier == Guid.Empty)
                    entity.CertificateIdentifier = UniqueIdentifier.Create();
                db.TCollegeCertificates.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TCollegeCertificate certificate)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(certificate).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
