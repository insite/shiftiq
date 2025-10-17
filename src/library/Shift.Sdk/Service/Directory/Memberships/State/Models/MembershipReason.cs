using System;

namespace InSite.Domain.Contacts
{
    public class MembershipReason
    {
        public Guid Identifier { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public DateTimeOffset Effective { get; set; }
        public DateTimeOffset? Expiry { get; set; }
        public string PersonOccupation { get; set; }
    }
}
