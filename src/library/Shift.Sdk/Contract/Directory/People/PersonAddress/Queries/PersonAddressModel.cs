using System;

namespace Shift.Contract
{
    public partial class PersonAddressModel
    {
        public Guid AddressId { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
    }
}