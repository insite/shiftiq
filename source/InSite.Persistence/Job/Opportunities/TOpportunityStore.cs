using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TOpportunityStore
    {
        public static void Insert(TOpportunity entity)
        {
            using (var db = new InternalDbContext())
            {
                db.TOpportunities.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(TOpportunity entity)
        {
            using (var db = new InternalDbContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Delete(Guid opportunityIdentifier)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TOpportunities.FirstOrDefault(x => x.OpportunityIdentifier == opportunityIdentifier);
                if (entity == null)
                    return;

                db.TOpportunities.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
