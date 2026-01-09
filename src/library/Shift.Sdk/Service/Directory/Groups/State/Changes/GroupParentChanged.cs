using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupParentChanged : Change
    {
        public Guid? Parent { get; }

        public GroupParentChanged(Guid? parent)
        {
            Parent = parent;
        }
    }
}
