using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class MembershipExpired : Change
    {
        public MembershipExpired(DateTimeOffset expiry)
        {
            Expiry = expiry;
        }

        public DateTimeOffset Expiry { get; }
    }
}