using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationFieldTextModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardValidationField Field { get; }

        public string Value { get; }

        public StandardValidationFieldTextModified(StandardValidationField field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}
