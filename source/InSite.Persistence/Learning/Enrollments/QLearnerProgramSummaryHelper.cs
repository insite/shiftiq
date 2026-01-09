using System.Linq;

namespace InSite.Persistence
{
    public static class QLearnerProgramSummaryHelper
    {
        public static IQueryable<QLearnerProgramSummary> Filter(this IQueryable<QLearnerProgramSummary> query, QLearnerProgramSummaryFilter filter)
        {
            if (filter.AsAt.Since.HasValue)
                query = query.Where(x => x.AsAt >= filter.AsAt.Since.Value);

            if (filter.AsAt.Before.HasValue)
                query = query.Where(x => x.AsAt < filter.AsAt.Before.Value);

            query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.User.Memberships.Any(m => m.GroupIdentifier == filter.GroupIdentifier));

            return query;
        }
    }
}
