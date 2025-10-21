using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCertificateLayoutHelper
    {
        public static IQueryable<TCertificateLayout> Filter(this IQueryable<TCertificateLayout> query, TCertificateLayoutFilter filter)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.Code.HasValue())
                query = query.Where(x => x.CertificateLayoutCode.Contains(filter.Code));

            if (filter.Data.HasValue())
                query = query.Where(x => x.CertificateLayoutData.Contains(filter.Data));

            return query;
        }
    }
}
