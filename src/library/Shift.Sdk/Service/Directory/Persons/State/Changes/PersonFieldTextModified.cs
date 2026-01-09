using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldTextModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public string Value { get; set; }

        public PersonFieldTextModified(PersonField personField, string value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
