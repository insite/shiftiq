using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldIntModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public int? Value { get; }

        public StandardFieldIntModified(StandardField field, int? value)
        {
            Field = field;
            Value = value;
        }
    }
}
