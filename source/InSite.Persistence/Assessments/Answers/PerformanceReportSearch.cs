using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;

using Shift.Common.Linq;

namespace InSite.Persistence
{
    public class PerformanceReportSearch : IPerformanceReportSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        public List<VPerformanceReport> GetReport(VPerformanceReportFilter filter)
        {
            using (var context = CreateContext())
            {
                var query = CreateQuery(filter, context);
                return query.OrderBy(r => r.AttemptIdentifier).ToList();
            }
        }

        private static IQueryable<VPerformanceReport> CreateQuery(VPerformanceReportFilter filter, InternalDbContext context)
        {
            var query = context.VPerformanceReports.Where(r =>
                r.OrganizationIdentifier == filter.OrganizationIdentifier
                && r.LearnerUserIdentifier == filter.LearnerUserIdentifier
            );

            if (filter.FormIdentifier.HasValue)
                query = query.Where(r => r.FormIdentifier == filter.FormIdentifier);

            if (filter.AttemptGradedSince.HasValue)
                query = query.Where(x => x.AttemptGraded >= filter.AttemptGradedSince.Value);

            if (filter.AttemptGradedBefore.HasValue)
                query = query.Where(x => x.AttemptGraded < filter.AttemptGradedBefore.Value);

            if (filter.AttemptIds != null && filter.AttemptIds.Length > 0)
                query = query.Where(x => filter.AttemptIds.Contains(x.AttemptIdentifier));

            return query;
        }
    }
}
