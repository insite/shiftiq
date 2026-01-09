using System;

namespace Shift.Contract
{
    public partial class SiteModel
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid SiteIdentifier { get; set; }

        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string SiteDomain { get; set; }
        public string SiteTitle { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
    }
}