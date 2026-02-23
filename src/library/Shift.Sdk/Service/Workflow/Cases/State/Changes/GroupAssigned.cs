using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class GroupAssigned : Change
    {
        public Guid Group { get; set; }
        public string Role { get; set; }

        public GroupAssigned(Guid group, string role)
        {
            Group = group;
            Role = role;
        }
    }
}
