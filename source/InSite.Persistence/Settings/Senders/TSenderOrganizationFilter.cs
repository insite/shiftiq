using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TSenderOrganizationFilter : Filter
    {
        public Guid? SenderIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
    }
}
