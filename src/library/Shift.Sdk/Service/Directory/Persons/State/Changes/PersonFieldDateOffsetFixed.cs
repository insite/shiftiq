using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldDateOffsetFixed : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public DateTimeOffset? Value { get; set; }

        public PersonFieldDateOffsetFixed(PersonField personField, DateTimeOffset? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
