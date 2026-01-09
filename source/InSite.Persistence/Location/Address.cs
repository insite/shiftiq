using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class Address
    {
        public Guid AddressIdentifier { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        public virtual ICollection<Person> BillingPersons { get; set; }
        public virtual ICollection<Person> HomePersons { get; set; }
        public virtual ICollection<Person> ShippingPersons { get; set; }
        public virtual ICollection<Person> WorkPersons { get; set; }

        [NotMapped]
        public bool IsEmpty =>
            City.IsEmpty()
            && Country.IsEmpty()
            && Description.IsEmpty()
            && PostalCode.IsEmpty()
            && Province.IsEmpty()
            && Street1.IsEmpty()
            && Street2.IsEmpty();

        public static readonly ICollection<string> DiffExclusions = new HashSet<string>
        {
            "IsSharedAddress",
            "IsSharedEmployerAddress",
            "IsSharedHomeAddress"
        };

        public Address()
        {
            BillingPersons = new HashSet<Person>();
            HomePersons = new HashSet<Person>();
            ShippingPersons = new HashSet<Person>();
            WorkPersons = new HashSet<Person>();
        }

        public Address Clone()
        {
            var clone = new Address();

            this.ShallowCopyTo(clone);

            return clone;
        }

        public void Trim()
        {
            City = City.Trim();
            Country = Country.Trim();
            Description = Description.Trim();
            PostalCode = PostalCode.Trim();
            Province = Province.Trim();
            Street1 = Street1.Trim();
            Street2 = Street2.Trim();
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

        #region Conversion

        #endregion
    }
}
