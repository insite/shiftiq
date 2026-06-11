using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class UserFieldTextModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserField UserField { get; set; }

        public string Value { get; set; }

        public UserFieldTextModified(UserField userField, string value)
        {
            UserField = userField;
            Value = value;
        }
    }
}
