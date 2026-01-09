using System;

namespace InSite.Persistence
{
    public class StandardOrganization
    {
        public Guid StandardIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual Standard Standard { get; set; }
        public virtual VOrganization Organization { get; set; }
    }
}
