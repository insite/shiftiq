using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.StandardValidations.Write
{
    public class ModifyStandardValidationFields : Command
    {
        [JsonProperty, JsonConverter(typeof(StandardValidationStateDictionaryConverter))]
        public Dictionary<StandardValidationField, object> Fields { get; private set; }

        [JsonConstructor]
        private ModifyStandardValidationFields()
        {

        }

        public ModifyStandardValidationFields(Guid standardId, params (StandardValidationField Field, object Value)[] values)
        {
            AggregateIdentifier = standardId;
            Fields = values.IsEmpty()
                ? new Dictionary<StandardValidationField, object>()
                : values.ToDictionary(x => x.Field, x => x.Value);
        }
    }
}
