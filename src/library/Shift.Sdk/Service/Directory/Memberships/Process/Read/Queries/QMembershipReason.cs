using System;

namespace InSite.Application.Contacts.Read
{
    public class QMembershipReason
    {
        public Guid MembershipIdentifier { get; set; }

        public Guid ReasonIdentifier { get; set; }
        public string ReasonType { get; set; }
        public string ReasonSubtype { get; set; }
        public DateTimeOffset ReasonEffective { get; set; }
        public DateTimeOffset? ReasonExpiry { get; set; }

        public string PersonOccupation { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }

        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public string LastChangeType { get; set; }

        public virtual QMembership Membership { get; set; }
    }
}
