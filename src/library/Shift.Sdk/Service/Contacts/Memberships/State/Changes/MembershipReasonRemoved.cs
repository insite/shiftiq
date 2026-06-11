using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipReasonRemoved : Change
    {
        public Guid ReasonIdentifier { get; }

        public MembershipReasonRemoved(Guid reasonIdentifier)
        {
            ReasonIdentifier = reasonIdentifier;
        }
    }
}
