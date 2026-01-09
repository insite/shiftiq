using System;

namespace Shift.Contract
{
    public partial class GroupAddressModel
    {
        public Guid AddressIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string AddressType { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}