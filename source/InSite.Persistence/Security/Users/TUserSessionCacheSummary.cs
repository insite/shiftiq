using System;

namespace InSite.Persistence
{
    [Serializable]
    public class TUserSessionCacheSummary
    {
        public string CompanyName { get; set; }
        public string CompanyTitle { get; set; }
        public DateTimeOffset SessionStarted { get; set; }
        public string OrganizationCode { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}