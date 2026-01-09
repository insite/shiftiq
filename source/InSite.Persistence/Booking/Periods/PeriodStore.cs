using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Records.Read;
using InSite.Domain.Records;

namespace InSite.Persistence
{
    public class PeriodStore : IPeriodStore
    {
        public void InsertPeriod(PeriodCreated e)
        {
            using (var db = CreateContext())
            {
                var entity = new QPeriod
                {
                    PeriodIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.Tenant,
                    PeriodName = e.Name,
                    PeriodStart = e.Start,
                    PeriodEnd = e.End
                };

                db.QPeriods.Add(entity);
                db.SaveChanges();
            }
        }

        public void UpdatePeriod(IChange e, Action<QPeriod> change)
        {
            using (var db = CreateContext())
            {
                var entity = db.QPeriods.Where(x => x.PeriodIdentifier == e.AggregateIdentifier).FirstOrDefault();

                change(entity);

                db.SaveChanges();
            }
        }

        public void DeletePeriod(PeriodDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QPeriods
                    .Where(x => x.PeriodIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QPeriods.Remove(entity);

                db.SaveChanges();
            }
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(false);
    }
}
