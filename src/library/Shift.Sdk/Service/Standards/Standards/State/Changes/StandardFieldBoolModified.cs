using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldBoolModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public bool? Value { get; }

        public StandardFieldBoolModified(StandardField field, bool? value)
        {
            Field = field;
            Value = value;
        }
    }
}
