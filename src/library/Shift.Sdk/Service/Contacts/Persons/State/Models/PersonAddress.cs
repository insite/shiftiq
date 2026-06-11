using System;

namespace InSite.Domain.Contacts
{
    public class PersonAddress
    {
        public Guid Identifier { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(City)
                && string.IsNullOrWhiteSpace(Country)
                && string.IsNullOrWhiteSpace(Description)
                && string.IsNullOrWhiteSpace(PostalCode)
                && string.IsNullOrWhiteSpace(Province)
                && string.IsNullOrWhiteSpace(Street1)
                && string.IsNullOrWhiteSpace(Street2);
        }

        public PersonAddress Clone()
        {
            return (PersonAddress)MemberwiseClone();
        }
    }
}
