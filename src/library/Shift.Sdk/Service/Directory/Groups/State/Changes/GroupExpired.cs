using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupExpired : Change
    {
        public DateTimeOffset Expiry { get; }

        public GroupExpired(DateTimeOffset expiry)
        {
            Expiry = expiry;
        }
    }
}
