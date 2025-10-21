using System;
using System.Linq;

namespace InSite.Persistence
{
    public class TEntity
    {
        public Guid EntityId { get; set; }

        public string CollectionSlug { get; set; }
        public string CollectionKey { get; set; }

        public string ComponentType { get; set; }
        public string ComponentName { get; set; }
        public string ComponentPart { get; set; }

        public string EntityName { get; set; }

        public string StorageKey { get; set; }
        public string StorageSchema { get; set; }
        public string StorageStructure { get; set; }
        public string StorageTable { get; set; }
        public string StorageTableRename { get; set; }

        public int StorageKeySize
            => StorageKey.Count(x => x == ',');

        public string StorageSchemaRename
            => StorageSchema != ComponentName.ToLower() ? ComponentName.ToLower() : null;
    }
}
