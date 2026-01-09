using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipReasonAdded : Change
    {
        public Guid ReasonIdentifier { get; }
        public string Type { get; }
        public string Subtype { get; }
        public DateTimeOffset Effective { get; }
        public DateTimeOffset? Expiry { get; }
        public string PersonOccupation { get; }

        public MembershipReasonAdded(Guid reasonIdentifier, string type, string subtype, DateTimeOffset effective, DateTimeOffset? expiry, string personOccupation)
        {
            ReasonIdentifier = reasonIdentifier;
            Type = type;
            Subtype = subtype;
            Effective = effective;
            Expiry = expiry;
            PersonOccupation = personOccupation;
        }
    }
}
