using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class PersonAddressModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AddressType AddressType { get; set; }
        public PersonAddress Address { get; set; }

        public PersonAddressModified(AddressType addressType, PersonAddress address)
        {
            AddressType = addressType;
            Address = address;
        }
    }
}
