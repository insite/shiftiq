using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldDateOffsetModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public DateTimeOffset? Value { get; set; }

        public PersonFieldDateOffsetModified(PersonField personField, DateTimeOffset? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
