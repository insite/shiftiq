using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SpecificationRetyped : Change
    {
        public Guid Specification { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationType Type { get; set; }

        public SpecificationRetyped(Guid spec, SpecificationType type)
        {
            Specification = spec;
            Type = type;
        }
    }
}
