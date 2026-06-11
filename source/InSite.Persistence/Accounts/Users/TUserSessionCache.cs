using System;

namespace InSite.Persistence
{
    public class TUserSessionCache
    {
        public Guid CacheIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public DateTimeOffset SessionStarted { get; set; }
    }
}