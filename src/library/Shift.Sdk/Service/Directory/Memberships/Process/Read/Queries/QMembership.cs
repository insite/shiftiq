using System;
using System.Collections.Generic;

namespace InSite.Application.Contacts.Read
{
    public class QMembership
    {
        public Guid GroupIdentifier { get; set; }
        public Guid MembershipIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string MembershipFunction { get; set; }

        public DateTimeOffset MembershipEffective { get; set; }
        public DateTimeOffset? MembershipExpiry { get; set; }

        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual QUser User { get; set; }
        public virtual QGroup Group { get; set; }

        public virtual ICollection<QMembershipReason> Reasons { get; set; } = new HashSet<QMembershipReason>();
    }
}