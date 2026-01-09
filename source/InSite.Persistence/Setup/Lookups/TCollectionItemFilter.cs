using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCollectionItemFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid? CollectionIdentifier { get; set; }
        public Guid? ExcludeItemIdentifier { get; set; }
        public string CollectionName { get; set; }
        public string ItemName { get; set; }
        public string ItemNameContains { get; set; }
        public string ItemFolder { get; set; }
        public int? ItemSequence { get; set; }
        public bool? HasOrganization { get; set; }
        public bool? HasFolder { get; set; }

        public TCollectionItemFilter Clone()
        {
            return (TCollectionItemFilter)MemberwiseClone();
        }
    }
}
