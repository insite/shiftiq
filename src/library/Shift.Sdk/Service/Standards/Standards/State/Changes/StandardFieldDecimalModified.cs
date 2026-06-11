using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldDecimalModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public StandardField Field { get; }

        public decimal? Value { get; }

        public StandardFieldDecimalModified(StandardField field, decimal? value)
        {
            Field = field;
            Value = value;
        }
    }
}
