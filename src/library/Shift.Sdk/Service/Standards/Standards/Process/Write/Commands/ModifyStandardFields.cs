using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Standards;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Standards.Write
{
    public class ModifyStandardFields : Command
    {
        [JsonProperty, JsonConverter(typeof(StandardStateDictionaryConverter))]
        public Dictionary<StandardField, object> Fields { get; private set; }

        [JsonConstructor]
        private ModifyStandardFields()
        {

        }

        public ModifyStandardFields(Guid standardId, params (StandardField Field, object Value)[] values)
        {
            AggregateIdentifier = standardId;
            Fields = values.IsEmpty()
                ? new Dictionary<StandardField, object>()
                : values.ToDictionary(x => x.Field, x => x.Value);
        }
    }
}
