using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardFieldsModified : Change
    {
        [JsonProperty, JsonConverter(typeof(StandardStateDictionaryConverter))]
        public Dictionary<StandardField, object> Fields { get; }

        public StandardFieldsModified(Dictionary<StandardField, object> fields)
        {
            Fields = fields;
        }
    }
}
