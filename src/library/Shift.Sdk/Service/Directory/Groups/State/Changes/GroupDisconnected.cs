using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class GroupDisconnected : Change
    {
        public Guid Group { get; }

        public GroupDisconnected(Guid group)
        {
            Group = group;
        }
    }
}
