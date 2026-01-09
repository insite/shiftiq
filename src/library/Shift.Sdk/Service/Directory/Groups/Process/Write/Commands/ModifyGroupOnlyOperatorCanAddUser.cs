using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class ModifyGroupOnlyOperatorCanAddUser : Command
    {
        public bool OnlyOperatorCanAddUser { get; }

        public ModifyGroupOnlyOperatorCanAddUser(Guid group, bool onlyOperatorCanAddUser)
        {
            AggregateIdentifier = group;
            OnlyOperatorCanAddUser = onlyOperatorCanAddUser;
        }
    }
}
