using System;

namespace Shift.Contract
{
    public class CreateSite
    {
        public Guid OrganizationId { get; set; }
        public Guid SiteId { get; set; }

        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string SiteDomain { get; set; }
        public string SiteTitle { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
    }
}