using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldDateModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public DateTime? Value { get; set; }

        public PersonFieldDateModified(PersonField personField, DateTime? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
