using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class PersonFieldIntModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PersonField PersonField { get; set; }

        public int? Value { get; set; }

        public PersonFieldIntModified(PersonField personField, int? value)
        {
            PersonField = personField;
            Value = value;
        }
    }
}
