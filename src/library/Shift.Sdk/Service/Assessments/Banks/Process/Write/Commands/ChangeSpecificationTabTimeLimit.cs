using System;

using Shift.Common.Timeline.Commands;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ChangeSpecificationTabTimeLimit : Command
    {
        public Guid Specification { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SpecificationTabTimeLimit TabTimeLimit { get; set; }

        public ChangeSpecificationTabTimeLimit(Guid bank, Guid spec, SpecificationTabTimeLimit tabTimeLimit)
        {
            AggregateIdentifier = bank;

            Specification = spec;
            TabTimeLimit = tabTimeLimit;
        }
    }
}
