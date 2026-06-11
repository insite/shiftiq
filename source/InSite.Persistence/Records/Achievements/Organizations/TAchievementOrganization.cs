using System;

namespace InSite.Persistence
{
    public class TAchievementOrganization
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual VOrganization Organization { get; set; }
    }
}
