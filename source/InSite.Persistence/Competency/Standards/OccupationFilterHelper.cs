using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class OccupationFilterHelper
    {
        public static IQueryable<Occupation> ApplyFilter(IQueryable<Occupation> query, OccupationFilter filter)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.JobTitle.IsNotEmpty())
                query = query.Where(x => x.JobTitle.Contains(filter.JobTitle));

            if (filter.Keyword.IsNotEmpty())
                query = query.Where(x =>
                    x.JobTitle.Contains(filter.Keyword) ||
                    x.Statements.Contains(filter.Keyword) ||
                    x.Purpose.Contains(filter.Keyword)
                );

            return query;
        }
    }
}
