using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class LocationFieldMask
    {
        public bool Description { get; set; }
        
        public bool Address1 { get; set; }
        public bool Address2 { get; set; }
        public bool City { get; set; }
        public bool Province { get; set; }
        public bool PostalCode { get; set; }
        public bool Country { get; set; }

        public bool Phone { get; set; }
        public bool Mobile { get; set; }

        public bool IsEmpty => !(Description || Address1 || Address2 || City || Province || PostalCode || Country || Phone || Mobile);

        public bool ShouldSerializeDescription() => Description;
        public bool ShouldSerializeAddress1() => Address1;
        public bool ShouldSerializeAddress2() => Address2;
        public bool ShouldSerializeCity() => City;
        public bool ShouldSerializeProvince() => Province;
        public bool ShouldSerializePostalCode() => PostalCode;
        public bool ShouldSerializeCountry() => Country;
        public bool ShouldSerializePhone() => Phone;
        public bool ShouldSerializeMobile() => Mobile;
    }
}