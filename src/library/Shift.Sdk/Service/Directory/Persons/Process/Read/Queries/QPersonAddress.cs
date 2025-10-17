using System;
using System.Collections.Generic;

using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    public class QPersonAddress
    {
        public Guid AddressIdentifier { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        public virtual ICollection<QPerson> BillingPersons { get; set; } = new HashSet<QPerson>();
        public virtual ICollection<QPerson> HomePersons { get; set; } = new HashSet<QPerson>();
        public virtual ICollection<QPerson> ShippingPersons { get; set; } = new HashSet<QPerson>();
        public virtual ICollection<QPerson> WorkPersons { get; set; } = new HashSet<QPerson>();

        public PersonAddress ToModel()
        {
            return new PersonAddress
            {
                Identifier = AddressIdentifier,
                City = City,
                Country = Country,
                Description = Description,
                PostalCode = PostalCode,
                Province = Province,
                Street1 = Street1,
                Street2 = Street2
            };
        }

        public void Trim()
        {
            City = City?.Trim();
            Country = Country?.Trim();
            Description = Description?.Trim();
            PostalCode = PostalCode?.Trim();
            Province = Province?.Trim();
            Street1 = Street1?.Trim();
            Street2 = Street2?.Trim();
        }

        public bool Equals(QGroupAddress address, StringComparison comparisonType)
        {
            return City.Equals(address.City, comparisonType)
                && Country.Equals(address.Country, comparisonType)
                && Description.Equals(address.Description, comparisonType)
                && PostalCode.Equals(address.PostalCode, comparisonType)
                && Province.Equals(address.Province, comparisonType)
                && Street1.Equals(address.Street1, comparisonType)
                && Street2.Equals(address.Street2, comparisonType);
        }

        public bool IsEmpty() =>
            City.IsEmpty()
            && Country.IsEmpty()
            && Description.IsEmpty()
            && PostalCode.IsEmpty()
            && Province.IsEmpty()
            && Street1.IsEmpty()
            && Street2.IsEmpty();

        public string ToHtml()
            => LocationHelper.ToHtml(Street1, Street2, City, Province, PostalCode, Country, null, null);

        public override string ToString()
            => LocationHelper.ToString(Street1, Street2, City, Province, PostalCode, Country, null, null, "\r\n");
    }
}
