using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TApplicationStore
    {
        public static void Insert(TApplication entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TApplications.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TApplication entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void DeleteJobApplication(Guid aplicationIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TApplications.FirstOrDefault(x => x.ApplicationIdentifier == aplicationIdentifier);
                if (entity == null)
                    return;

                db.TApplications.Remove(entity);
                db.SaveChanges();
            }
        }

        public static void DeleteJByOpportunity(Guid opportunityIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entities = db.TApplications.Where(x => x.OpportunityIdentifier == opportunityIdentifier).ToList();

                if (entities.Count > 0)
                {
                    db.TApplications.RemoveRange(entities);
                    db.SaveChanges();
                }
            }
        }
    }
}
