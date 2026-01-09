using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldTextModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public string Value { get; }

        public StandardFieldTextModified(StandardField field, string value)
        {
            Field = field;
            Value = value;
        }
    }
}
