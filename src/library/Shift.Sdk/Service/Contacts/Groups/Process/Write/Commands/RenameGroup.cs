using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class RenameGroup : Command
    {
        public string Type { get; }
        public string Name { get; }

        public RenameGroup(Guid group, string type, string name)
        {
            AggregateIdentifier = group;
            Type = type;
            Name = name;
        }
    }
}
