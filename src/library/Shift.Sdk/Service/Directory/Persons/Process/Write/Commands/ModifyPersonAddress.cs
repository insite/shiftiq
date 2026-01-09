using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.People.Write
{
    public class ModifyPersonAddress : Command
    {
        public AddressType AddressType { get; set; }
        public PersonAddress Address { get; set; }

        public ModifyPersonAddress(Guid personId, AddressType addressType, PersonAddress address)
        {
            AggregateIdentifier = personId;
            AddressType = addressType;
            Address = address;
        }
    }
}
