using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCertificateLayoutFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string Code { get; set; }
        public string Data { get; set; }
    }
}
