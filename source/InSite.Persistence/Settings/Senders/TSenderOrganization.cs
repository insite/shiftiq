using System;

namespace InSite.Persistence
{
    public class TSenderOrganization
    {
        public Guid SenderIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid JoinIdentifier { get; set; }

        public virtual TSender Sender { get; set; }
        public virtual VOrganization Organization { get; set; }
    }
}
