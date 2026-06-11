using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupExpiryChanged : Change
    {
        public DateTimeOffset? Expiry { get; }

        public GroupExpiryChanged(DateTimeOffset? expiry)
        {
            Expiry = expiry;
        }
    }
}
