using System.Collections.Generic;
using System.Linq;

using InSite.Application.Events.Read;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class VExamEventScheduleSearch
    {
        public static List<AccommodationSummary> SelectAccommodationSummaries(VExamEventScheduleFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var eventQuery = CreateEventQueryByFilter(filter, db);

                return eventQuery.Join(db.Registrations,
                        e => e.EventIdentifier,
                        r => r.EventIdentifier,
                        (e, r) => r
                    )
                    .Join(db.Accommodations,
                        r => r.RegistrationIdentifier,
                        a => a.RegistrationIdentifier,
                        (r, a) => new
                        {
                            r.EventIdentifier,
                            a.AccommodationType
                        }
                    )
                    .GroupBy(x => new { x.EventIdentifier, x.AccommodationType })
                    .Select(x => new AccommodationSummary
                    {
                        EventIdentifier = x.Key.EventIdentifier,
                        AccommodationType = x.Key.AccommodationType,
                        RegistrationCount = x.Count()
                    })
                    .ToList();
            }
        }

        public static int CountByFilter(VExamEventScheduleFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db).Count();
            }
        }

        public static List<VExamEventSchedule> SelectByFilter(VExamEventScheduleFilter filter)
        {
            var sortExpression = $"{nameof(VExamEventSchedule.EventScheduledStart)} desc";

            if (!string.IsNullOrEmpty(filter.SortByColumn))
                sortExpression = filter.SortByColumn;

            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).OrderBy(sortExpression).ToList();
        }

        private static IQueryable<VExamEventSchedule> CreateQueryByFilter(VExamEventScheduleFilter filter, InternalDbContext db)
        {
            var query = db.VExamEventSchedules
                .AsQueryable()
                .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ScheduledSince.HasValue)
                query = query.Where(x => x.EventScheduledStart >= filter.ScheduledSince.Value);

            if (filter.ScheduledBefore.HasValue)
                query = query.Where(x => x.EventScheduledStart < filter.ScheduledBefore.Value);

            return query;
        }

        private static IQueryable<QEvent> CreateEventQueryByFilter(VExamEventScheduleFilter filter, InternalDbContext db)
        {
            var query = db.Events
                .AsNoTracking()
                .Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.ScheduledSince.HasValue)
                query = query.Where(x => x.EventScheduledStart >= filter.ScheduledSince.Value);

            if (filter.ScheduledBefore.HasValue)
                query = query.Where(x => x.EventScheduledStart < filter.ScheduledBefore.Value);

            return query;
        }
    }
}
