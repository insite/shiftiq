using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TSenderHelper
    {
        public static IQueryable<TSender> Filter(this IQueryable<TSender> query, TSenderFilter filter)
        {
            if (filter.SenderType.HasValue())
                query = query.Where(x => x.SenderType.Contains(filter.SenderType));

            if (filter.SenderNickname.HasValue())
                query = query.Where(x => x.SenderNickname.Contains(filter.SenderNickname));

            if (filter.SenderName.HasValue())
                query = query.Where(x => x.SenderName.Contains(filter.SenderName));

            if (filter.SenderEmail.HasValue())
                query = query.Where(x => x.SenderEmail.Contains(filter.SenderEmail));

            if (filter.SystemMailbox.HasValue())
                query = query.Where(x => x.SystemMailbox.Contains(filter.SystemMailbox));

            if (filter.SenderEnabled.HasValue)
                query = query.Where(x => x.SenderEnabled == filter.SenderEnabled);

            if (filter.CompanyAddress.HasValue())
                query = query.Where(x => x.CompanyAddress.Contains(filter.CompanyAddress));

            if (filter.CompanyCity.HasValue())
                query = query.Where(x => x.CompanyCity.Contains(filter.CompanyCity));

            if (filter.CompanyPostalCode.HasValue())
                query = query.Where(x => x.CompanyPostalCode.Contains(filter.CompanyPostalCode));

            if (filter.CompanyCountry.HasValue())
                query = query.Where(x => x.CompanyCountry.Contains(filter.CompanyCountry));

            return query;
        }
    }
}
