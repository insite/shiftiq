using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Records.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class PeriodSearch : IPeriodSearch
    {
        public QPeriod GetPeriod(Guid periodIdentifier, params Expression<Func<QPeriod, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QPeriods
                    .Where(x => x.PeriodIdentifier == periodIdentifier)
                    .ApplyIncludes(includes);

                query = query.OrderByDescending(x => x.PeriodStart);

                return query.FirstOrDefault();
            }
        }

        public QPeriod[] GetPeriods(IEnumerable<Guid> periodIdentifiers, params Expression<Func<QPeriod, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = db.QPeriods
                    .Where(x => periodIdentifiers.Contains(x.PeriodIdentifier))
                    .ApplyIncludes(includes);

                return query.ToArray();
            }
        }

        public bool PeriodExists(QPeriodFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Any();
        }

        public int CountPeriods(QPeriodFilter filter)
        {
            using (var db = CreateContext())
                return CreateQuery(filter, db).Count();
        }

        public List<QPeriod> GetPeriods(QPeriodFilter filter, params Expression<Func<QPeriod, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                var query = CreateQuery(filter, db).ApplyIncludes(includes);

                query = !string.IsNullOrEmpty(filter.OrderBy)
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderByDescending(x => x.PeriodStart);

                return query.ApplyPaging(filter).ToList();
            }
        }

        private static IQueryable<QPeriod> CreateQuery(QPeriodFilter filter, InternalDbContext db)
        {
            var query = db.QPeriods.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (!string.IsNullOrEmpty(filter.ExactPeriodName))
                query = query.Where(x => x.PeriodName == filter.ExactPeriodName);

            if (!string.IsNullOrEmpty(filter.PeriodName))
                query = query.Where(x => x.PeriodName.Contains(filter.PeriodName));

            if (filter.PeriodSince.HasValue)
                query = query.Where(x => x.PeriodStart >= filter.PeriodSince.Value);

            if (filter.PeriodBefore.HasValue)
                query = query.Where(x => x.PeriodEnd < filter.PeriodBefore.Value);

            if (filter.Identifiers.IsNotEmpty())
                query = query.Where(x => filter.Identifiers.Contains(x.PeriodIdentifier));

            if (filter.ExcludeIdentifiers.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeIdentifiers.Contains(x.PeriodIdentifier));

            return query;
        }

        private InternalDbContext CreateContext()
            => new InternalDbContext(false);
    }
}
