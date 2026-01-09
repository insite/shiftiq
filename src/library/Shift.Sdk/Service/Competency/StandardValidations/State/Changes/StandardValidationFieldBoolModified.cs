using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationFieldBoolModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardValidationField Field { get; }

        public bool? Value { get; }

        public StandardValidationFieldBoolModified(StandardValidationField field, bool? value)
        {
            Field = field;
            Value = value;
        }
    }
}
