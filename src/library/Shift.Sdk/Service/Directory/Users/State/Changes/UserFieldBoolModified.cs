using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class UserFieldBoolModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserField UserField { get; set; }

        public bool? Value { get; set; }

        public UserFieldBoolModified(UserField userField, bool? value)
        {
            UserField = userField;
            Value = value;
        }
    }
}
