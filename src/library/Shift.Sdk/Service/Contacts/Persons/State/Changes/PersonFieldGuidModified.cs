using System;
using System.ComponentModel;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldGuidModified : Change
    {
        [DefaultValue(PersonField.EmployerGroupIdentifier)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public PersonField PersonField { get; set; }

        public Guid? Value { get; set; }

        public PersonFieldGuidModified(PersonField personField, Guid? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
