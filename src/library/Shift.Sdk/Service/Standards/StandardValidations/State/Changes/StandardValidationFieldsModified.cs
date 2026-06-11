using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationFieldsModified : Change
    {
        [JsonProperty, JsonConverter(typeof(StandardValidationStateDictionaryConverter))]
        public Dictionary<StandardValidationField, object> Fields { get; }

        public StandardValidationFieldsModified(Dictionary<StandardValidationField, object> fields)
        {
            Fields = fields;
        }
    }
}
