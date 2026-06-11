using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class AddMembershipReason : Command
    {
        public Guid ReasonIdentifier { get; }
        public string Type { get; }
        public string Subtype { get; }
        public DateTimeOffset Effective { get; }
        public DateTimeOffset? Expiry { get; }
        public string PersonOccupation { get; }

        public AddMembershipReason(Guid membershipIdentifier, Guid reasonIdentifier, string type, string subtype, DateTimeOffset effective, DateTimeOffset? expiry, string personOccupation)
        {
            AggregateIdentifier = membershipIdentifier;

            ReasonIdentifier = reasonIdentifier;
            Type = type;
            Subtype = subtype;
            Effective = effective;
            Expiry = expiry;
            PersonOccupation = personOccupation;
        }
    }
}
