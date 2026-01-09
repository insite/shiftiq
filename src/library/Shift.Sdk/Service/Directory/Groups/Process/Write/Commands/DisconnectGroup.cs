using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class DisconnectGroup : Command
    {
        public Guid ConnectedGroup { get; }

        public DisconnectGroup(Guid group, Guid connectedGroup)
        {
            AggregateIdentifier = group;
            ConnectedGroup = connectedGroup;
        }
    }
}
