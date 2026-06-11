using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using InSite.Application.Utility.Read;

namespace InSite.Persistence.Platform.Collections
{
    public class CollectionSearch : ICollectionSearch
    {
        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(false);
        }

        public Dictionary<Guid, string> Select(HashSet<Guid> itemIdentifiers)
        {
            if(itemIdentifiers == null ||  itemIdentifiers.Count == 0)
                return null;

            using (var db = CreateContext())
            {
                return db.TCollectionItems.AsQueryable().AsNoTracking()
                    .Where(x => itemIdentifiers.Contains(x.ItemIdentifier))
                    .ToDictionary(x => x.ItemIdentifier, x => x.ItemName);
            }
        }
    }
}
