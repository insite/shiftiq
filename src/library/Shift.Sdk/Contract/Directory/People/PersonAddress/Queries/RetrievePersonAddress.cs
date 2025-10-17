using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePersonAddress : Query<PersonAddressModel>
    {
        public Guid AddressIdentifier { get; set; }
    }
}