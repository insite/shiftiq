using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class GroupAddressChanged : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AddressType Type { get; }

        public GroupAddress Address { get; }

        public GroupAddressChanged(AddressType type, GroupAddress address)
        {
            Type = type;
            Address = address;
        }
    }
}
