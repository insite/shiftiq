using System.Linq;

namespace InSite.Persistence
{
    public static class TSenderOrganizationHelper
    {
        public static IQueryable<TSenderOrganization> Filter(this IQueryable<TSenderOrganization> query, TSenderOrganizationFilter filter)
        {
            if (filter.SenderIdentifier.HasValue)
                query = query.Where(x => x.SenderIdentifier == filter.SenderIdentifier);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            return query;
        }
    }
}
