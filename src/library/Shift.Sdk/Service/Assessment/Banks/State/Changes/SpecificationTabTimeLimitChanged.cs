using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class SpecificationTabTimeLimitChanged : Change
    {
        public Guid Specification { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationTabTimeLimit TabTimeLimit { get; set; }

        public SpecificationTabTimeLimitChanged(Guid spec, SpecificationTabTimeLimit tabTimeLimit)
        {
            Specification = spec;
            TabTimeLimit = tabTimeLimit;
        }
    }
}
