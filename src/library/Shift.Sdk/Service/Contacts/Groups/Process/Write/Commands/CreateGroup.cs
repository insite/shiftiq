using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Groups.Write
{
    public class CreateGroup : Command
    {
        public Guid Tenant { get; }
        public string Type { get; }
        public string Name { get; }

        public CreateGroup(Guid group, Guid tenant, string type, string name)
        {
            AggregateIdentifier = group;
            Tenant = tenant;
            Type = type;
            Name = name;
        }
    }
}
