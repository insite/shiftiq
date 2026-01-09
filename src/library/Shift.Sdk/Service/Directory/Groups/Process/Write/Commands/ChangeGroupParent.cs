using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupParent : Command
    {
        public Guid? Parent { get; }

        public ChangeGroupParent(Guid group, Guid? parent)
        {
            AggregateIdentifier = group;
            Parent = parent;
        }
    }
}
