using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCatalogFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public string CatalogName { get; set; }

        public TCatalogFilter Clone()
        {
            return (TCatalogFilter)MemberwiseClone();
        }
    }
}
