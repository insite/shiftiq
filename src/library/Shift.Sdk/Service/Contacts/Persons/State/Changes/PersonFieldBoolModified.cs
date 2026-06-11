using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldBoolModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public bool? Value { get; set; }

        public PersonFieldBoolModified(PersonField personField, bool? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
