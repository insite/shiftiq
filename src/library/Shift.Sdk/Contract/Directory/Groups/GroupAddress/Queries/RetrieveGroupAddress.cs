using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroupAddress : Query<GroupAddressModel>
    {
        public Guid AddressIdentifier { get; set; }
    }
}