using System.Collections.Generic;
using System.Linq;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class ImpersonationSearch
    {
        public static int Count(ImpersonationFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByFilter(filter, db).Count();
        }

        public static List<ImpersonationSummary> Select(ImpersonationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByFilter(filter, db)
                    .OrderByDescending(x => x.ImpersonationStarted)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<ImpersonationSummary> CreateQueryByFilter(ImpersonationFilter filter, InternalDbContext db)
        {
            var query = db.ImpersonationSummaries
                .Where(x => !x.ImpersonatorIsCloaked)
                .AsQueryable();

            if (filter.SinceDate.HasValue)
                query = query.Where(x => x.ImpersonationStarted >= filter.SinceDate.Value);

            if (filter.BeforeDate.HasValue)
                query = query.Where(x => x.ImpersonationStarted < filter.BeforeDate.Value);

            return query;
        }
    }
}