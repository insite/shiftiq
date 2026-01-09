using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldGuidModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public Guid? Value { get; }

        public StandardFieldGuidModified(StandardField field, Guid? value)
        {
            Field = field;
            Value = value;
        }
    }
}
