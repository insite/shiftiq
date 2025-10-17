using System;

namespace InSite.Application.Contacts.Read
{
    public class VMembership
    {
        public Guid GroupIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string MembershipType { get; set; }

        public DateTimeOffset Assigned { get; set; }

        public virtual QGroup Group { get; set; }
        public virtual VUser User { get; set; }
    }
}
