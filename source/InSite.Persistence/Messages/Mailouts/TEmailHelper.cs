using System.Linq;

using InSite.Application.Messages.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TEmailHelper
    {
        public static IQueryable<VMailout> Filter(this IQueryable<VMailout> query, TEmailFilter filter)
        {
            if (filter.EmailSubject.HasValue())
                query = query.Where(x => x.ContentSubject.Contains(filter.EmailSubject));

            if (filter.MessageIdentifier.HasValue)
                query = query.Where(x => x.MessageIdentifier == filter.MessageIdentifier);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            return query;
        }
    }
}
