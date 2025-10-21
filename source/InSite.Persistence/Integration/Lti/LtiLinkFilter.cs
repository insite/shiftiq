using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class LtiLinkFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public string Publisher { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Subtype { get; set; }
        public string Code { get; set; }
    }
}
