using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCollectionItemHelper
    {
        public static IQueryable<TCollectionItem> Filter(this IQueryable<TCollectionItem> query, TCollectionItemFilter filter)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.GroupIdentifier.Value);

            if (filter.CollectionIdentifier.HasValue)
                query = query.Where(x => x.CollectionIdentifier == filter.CollectionIdentifier.Value);

            if (filter.ExcludeItemIdentifier.HasValue)
                query = query.Where(x => x.ItemIdentifier != filter.ExcludeItemIdentifier.Value);

            if (filter.CollectionName.IsNotEmpty())
                query = query.Where(x => x.Collection.CollectionName == filter.CollectionName);

            if (filter.ItemName.IsNotEmpty())
                query = query.Where(x => x.ItemName == filter.ItemName);

            if (filter.ItemNameContains.IsNotEmpty())
                query = query.Where(x => x.ItemName.Contains(filter.ItemNameContains));

            if (filter.ItemFolder.IsNotEmpty())
                query = query.Where(x => x.ItemFolder == filter.ItemFolder);

            if (filter.ItemSequence.HasValue)
                query = query.Where(x => x.ItemSequence == filter.ItemSequence.Value);

            if (filter.HasOrganization.HasValue)
            {
                if (filter.HasOrganization.Value)
                    query = query.Where(x => x.OrganizationIdentifier.HasValue);
                else
                    query = query.Where(x => !x.OrganizationIdentifier.HasValue);
            }

            if (filter.HasFolder.HasValue)
            {
                if (filter.HasFolder.Value)
                    query = query.Where(x => x.ItemFolder != null);
                else
                    query = query.Where(x => x.ItemFolder == null);
            }

            return query;
        }
    }
}
