using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationFieldDateOffsetModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardValidationField Field { get; }

        public DateTimeOffset? Value { get; }

        public StandardValidationFieldDateOffsetModified(StandardValidationField field, DateTimeOffset? value)
        {
            Field = field;
            Value = value;
        }
    }
}
