using System;
using System.ComponentModel.DataAnnotations.Schema;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    public class QGroupAddress
    {
        public Guid AddressIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }

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

        public virtual QGroup Group { get; set; }

        [NotMapped]
        public bool IsEmpty =>
            !HasAddress && !HasCoordinates;

        [NotMapped]
        public bool HasAddress =>
            City.IsNotEmpty()
            || Country.IsNotEmpty()
            || Description.IsNotEmpty()
            || PostalCode.IsNotEmpty()
            || Province.IsNotEmpty()
            || Street1.IsNotEmpty()
            || Street2.IsNotEmpty();

        [NotMapped]
        public bool HasCoordinates =>
            Latitude.HasValue && Longitude.HasValue;
    }
}
