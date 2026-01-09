using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class UserFieldDateOffsetModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public UserField UserField { get; set; }

        public DateTimeOffset? Value { get; set; }

        public UserFieldDateOffsetModified(UserField userField, DateTimeOffset? value)
        {
            UserField = userField;
            Value = value;
        }
    }
}
