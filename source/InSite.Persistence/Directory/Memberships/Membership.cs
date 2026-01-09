using System;

namespace InSite.Persistence
{
    public class Membership
    {
        public Guid MembershipIdentifier { get; set; }

        public Guid GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string MembershipType { get; set; }

        public DateTimeOffset Assigned { get; set; }
        public DateTimeOffset? MembershipExpiry { get; set; }

        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual User User { get; set; }
        public virtual VGroupDetail Group { get; set; }
    }
}
