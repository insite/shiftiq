using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class AssignGroup : Command
    {
        public Guid Group { get; set; }
        public string Role { get; set; }

        public AssignGroup(Guid aggregate, Guid group, string role)
        {
            AggregateIdentifier = aggregate;
            Group = group;
            Role = role;
        }
    }
}
