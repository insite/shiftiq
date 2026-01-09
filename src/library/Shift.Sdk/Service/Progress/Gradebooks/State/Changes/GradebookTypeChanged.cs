using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class GradebookTypeChanged : Change
    {
        public GradebookTypeChanged(GradebookType type, Guid? framework)
        {
            Type = type;
            Framework = framework;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public GradebookType Type { get; set; }

        public Guid? Framework { get; set; }
    }
}
