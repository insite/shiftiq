using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ConnectGroup : Command
    {
        public Guid Parent { get; }

        public ConnectGroup(Guid child, Guid parent)
        {
            AggregateIdentifier = child;
            Parent = parent;
        }
    }
}
