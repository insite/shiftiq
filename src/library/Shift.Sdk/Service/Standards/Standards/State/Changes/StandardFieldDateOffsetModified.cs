using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldDateOffsetModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public DateTimeOffset? Value { get; }

        public StandardFieldDateOffsetModified(StandardField field, DateTimeOffset? value)
        {
            Field = field;
            Value = value;
        }
    }
}
