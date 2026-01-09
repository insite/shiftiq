using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationFieldGuidModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardValidationField Field { get; }

        public Guid? Value { get; }

        public StandardValidationFieldGuidModified(StandardValidationField field, Guid? value)
        {
            Field = field;
            Value = value;
        }
    }
}
