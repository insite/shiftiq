using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupExpiry : Command
    {
        public DateTimeOffset? Expiry { get; }

        public ChangeGroupExpiry(Guid group, DateTimeOffset? expiry)
        {
            AggregateIdentifier = group;
            Expiry = expiry;
        }
    }
}