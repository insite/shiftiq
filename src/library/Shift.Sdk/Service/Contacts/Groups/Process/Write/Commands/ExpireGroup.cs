using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ExpireGroup : Command
    {
        public DateTimeOffset Expiry { get; }

        public ExpireGroup(Guid group, DateTimeOffset expiry)
        {
            AggregateIdentifier = group;
            Expiry = expiry;
        }
    }
}