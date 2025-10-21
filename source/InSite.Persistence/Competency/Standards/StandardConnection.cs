using System;

namespace InSite.Persistence
{
    public class StandardConnection
    {
        public String ConnectionType { get; set; }
        public Guid FromStandardIdentifier { get; set; }
        public Guid ToStandardIdentifier { get; set; }

        public virtual Standard FromStandard { get; set; }
        public virtual Standard ToStandard { get; set; }
    }
}
