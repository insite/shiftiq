using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TReportStore
    {
        public static void Insert(TReport entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TReports.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TReport entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid report)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TReports.FirstOrDefault(x => x.ReportIdentifier == report);
                if (entity == null)
                    return;
                db.TReports.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
