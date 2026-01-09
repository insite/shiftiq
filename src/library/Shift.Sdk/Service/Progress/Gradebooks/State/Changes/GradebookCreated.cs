using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class GradebookCreated : Change
    {
        public GradebookCreated(Guid tenant, string name, GradebookType type, Guid? @event, Guid? achievement, Guid? framework)
        {
            Tenant = tenant;
            Name = name;
            Type = type;
            Event = @event;
            Achievement = achievement;
            Framework = framework;
        }

        public Guid Tenant { get; set; }
        public string Name { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GradebookType Type { get; set; }

        public Guid? Event { get; set; }
        public Guid? Achievement { get; set; }
        public Guid? Framework { get; set; }
    }
}