using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class UserFieldIntModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserField UserField { get; set; }

        public int? Value { get; set; }

        public UserFieldIntModified(UserField userField, int? value)
        {
            UserField = userField;
            Value = value;
        }
    }
}
