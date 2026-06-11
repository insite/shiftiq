using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class GroupOnlyOperatorCanAddUserModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool OnlyOperatorCanAddUser { get; set; }

        public GroupOnlyOperatorCanAddUserModified(bool onlyOperatorCanAddUser)
        {
            OnlyOperatorCanAddUser = onlyOperatorCanAddUser;
        }
    }
}
