using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipExpiryModified : Change
    {
        public DateTimeOffset? Expiry { get; }

        public MembershipExpiryModified(DateTimeOffset? expiry)
        {
            Expiry = expiry;
        }
    }
}