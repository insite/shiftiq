using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class RemoveMembershipReason : Command
    {
        public Guid ReasonIdentifier { get; }

        public RemoveMembershipReason(Guid membershipIdentifier, Guid reasonIdentifier)
        {
            AggregateIdentifier = membershipIdentifier;

            ReasonIdentifier = reasonIdentifier;
        }
    }
}
