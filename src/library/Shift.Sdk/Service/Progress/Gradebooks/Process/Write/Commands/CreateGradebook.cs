using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Gradebooks.Write
{
    public class CreateGradebook : Command
    {
        public CreateGradebook(Guid record, Guid tenant, string name, GradebookType type, Guid? @event, Guid? achievement, Guid? framework)
        {
            AggregateIdentifier = record;
            Tenant = tenant;
            Name = name;
            Type = type;
            Event = @event;
            Achievement = achievement;
            Framework = framework;
        }

        public Guid Tenant { get; set; }
        public string Name { get; set; }
        public GradebookType Type { get; set; }
        public Guid? Event { get; set; }
        public Guid? Achievement { get; set; }
        public Guid? Framework { get; set; }
    }
}