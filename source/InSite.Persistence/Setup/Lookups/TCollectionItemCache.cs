using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCollectionItemCache
    {
        private class CacheData
        {
            public TCollectionItem[] Items { get; set; }
            public Dictionary<Guid, TCollectionItem> ItemsById { get; set; }
            public Dictionary<Guid, TCollectionItem[]> ItemsByCollectionId { get; set; }
            public Dictionary<string, TCollectionItem[]> ItemsByCollectionName { get; set; }
        }

        private static CacheData _data;

        static TCollectionItemCache() => Refresh();

        public static void Refresh()
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var items = db.TCollectionItems.AsNoTracking()
                    .Include(x => x.Collection)
                    .Include(x => x.Organization)
                    .OrderBy(x => x.ItemSequence).ThenBy(x => x.ItemName)
                    .ToArray();

                _data = new CacheData
                {
                    Items = items,
                    ItemsById = items.ToDictionary(x => x.ItemIdentifier, x => x),
                    ItemsByCollectionId = items.Where(x => x.Collection != null)
                        .GroupBy(x => x.Collection.CollectionIdentifier)
                        .ToDictionary(x => x.Key, x => x.ToArray()),
                    ItemsByCollectionName = items.Where(x => x.Collection != null)
                        .GroupBy(x => x.Collection.CollectionName)
                        .ToDictionary(x => x.Key, x => x.ToArray(), StringComparer.OrdinalIgnoreCase)
                };
            }
        }

        public static TCollectionItem Select(Guid id)
        {
            return _data.ItemsById.GetOrDefault(id);
        }

        public static TCollectionItem Select(int number)
        {
            return _data.Items.FirstOrDefault(x => x.ItemNumber == number);
        }

        public static string GetName(Guid? id)
        {
            return id.HasValue ? Select(id.Value)?.ItemName : null;
        }

        public static IQueryable<TCollectionItem> Query(TCollectionItemFilter filter)
        {
            var innerFilter = filter.Clone();
            IEnumerable<TCollectionItem> items = _data.Items;

            if (filter.CollectionIdentifier.HasValue)
            {
                items = _data.ItemsByCollectionId.GetOrDefault(filter.CollectionIdentifier.Value)
                    ?? Enumerable.Empty<TCollectionItem>();
                innerFilter.CollectionIdentifier = null;
                innerFilter.CollectionName = null;
            }
            else if (filter.CollectionName.IsNotEmpty())
            {
                items = _data.ItemsByCollectionName.GetOrDefault(filter.CollectionName)
                    ?? Enumerable.Empty<TCollectionItem>();
                innerFilter.CollectionName = null;
            }

            var query = items.AsQueryable();

            if (filter.ItemName.IsNotEmpty())
            {
                query = query.Where(x => string.Equals(x.ItemName, filter.ItemName, StringComparison.OrdinalIgnoreCase));
                innerFilter.ItemName = null;
            }

            if (filter.ItemFolder.IsNotEmpty())
            {
                query = query.Where(x => string.Equals(x.ItemFolder, filter.ItemFolder, StringComparison.OrdinalIgnoreCase));
                innerFilter.ItemFolder = null;
            }

            return query.Filter(innerFilter);
        }

        public static List<TCollectionItem> Select(TCollectionItemFilter filter) => Query(filter).ToList();

        public static TCollectionItem SelectFirst(TCollectionItemFilter filter) => Query(filter).FirstOrDefault();

        public static bool Exists(TCollectionItemFilter filter) => Query(filter).Any();

        public static int Count(TCollectionItemFilter filter) => Query(filter).Count();
    }
}
