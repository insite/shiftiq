using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ModifyAllowJoinGroupUsingLink : Command
    {
        public bool AllowJoinGroupUsingLink { get; }

        public ModifyAllowJoinGroupUsingLink(Guid group, bool allowJoinGroupUsingLink)
        {
            AggregateIdentifier = group;
            AllowJoinGroupUsingLink = allowJoinGroupUsingLink;
        }
    }
}
