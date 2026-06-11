using System.Linq;

namespace InSite.Persistence
{
    public static class TPersonFieldExtensions
    {
        public static IQueryable<TPersonField> Filter(this IQueryable<TPersonField> query, TPersonFieldFilter filter)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            if (!string.IsNullOrEmpty(filter.FieldName))
                query = query.Where(x => x.FieldName == filter.FieldName);

            return query;
        }
    }
}
