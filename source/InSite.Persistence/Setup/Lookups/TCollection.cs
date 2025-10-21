using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TCollection
    {
        public Guid CollectionIdentifier { get; set; }

        public string CollectionName { get; set; }
        public string CollectionPackage { get; set; }
        public string CollectionProcess { get; set; }
        public string CollectionReferences { get; set; }
        public string CollectionTool { get; set; }
        public string CollectionType { get; set; }

        public virtual ICollection<TCollectionItem> Items { get; set; }

        public TCollection()
        {
            Items = new HashSet<TCollectionItem>();
        }
    }
}