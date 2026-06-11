using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupAddress : Command
    {
        public AddressType Type { get; }
        public GroupAddress Address { get; }

        public ChangeGroupAddress(Guid group, AddressType type, GroupAddress address)
        {
            AggregateIdentifier = group;
            Type = type;
            Address = address.Clone(true);
        }
    }
}
