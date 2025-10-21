using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TEntityFilter : Filter
    {
        public string Keyword { get; set; }

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
    }
}
