using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    public class AllowJoinGroupUsingLinkModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool AllowJoinGroupUsingLink { get; set; }

        public AllowJoinGroupUsingLinkModified(bool allowJoinGroupUsingLink)
        {
            AllowJoinGroupUsingLink = allowJoinGroupUsingLink;
        }
    }
}
