using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupCapacity : Command
    {
        public int? Capacity { get; }

        public ChangeGroupCapacity(Guid group, int? capacity)
        {
            AggregateIdentifier = group;
            Capacity = capacity;
        }
    }
}
