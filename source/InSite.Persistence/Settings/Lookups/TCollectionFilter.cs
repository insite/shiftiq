using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCollectionFilter : Filter
    {
        public Guid? ExcludeCollectionIdentifier { get; set; }
        public string CollectionName { get; set; }
        public string CollectionTool { get; set; }
        public string CollectionProcess { get; set; }
        public string CollectionType { get; set; }
    }
}
