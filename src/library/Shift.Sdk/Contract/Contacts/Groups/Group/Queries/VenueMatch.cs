using System;

namespace Shift.Contract
{
    public class VenueMatch
    {
        public Guid VenueIdentifier { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }

        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
