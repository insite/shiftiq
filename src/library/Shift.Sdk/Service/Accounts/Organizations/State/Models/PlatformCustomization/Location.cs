using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class Location
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public LocationType LocationType { get; set; }

        public string Description { get; set; }
        public string TimeZone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string TollFree { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }

        public bool IsEmpty => !(
               ShouldSerializeLocationType()
            || ShouldSerializeDescription()
            || ShouldSerializeTimeZone()
            || ShouldSerializeStreet()
            || ShouldSerializeCity()
            || ShouldSerializeProvince()
            || ShouldSerializePostalCode()
            || ShouldSerializeCountry()
            || ShouldSerializePhone()
            || ShouldSerializeTollFree()
            || ShouldSerializeMobile()
            || ShouldSerializeFax()
            || ShouldSerializeEmail());

        public bool ShouldSerializeLocationType() => LocationType != LocationType.None;
        public bool ShouldSerializeDescription() => !string.IsNullOrEmpty(Description);
        public bool ShouldSerializeTimeZone() => !string.IsNullOrEmpty(TimeZone);
        public bool ShouldSerializeStreet() => !string.IsNullOrEmpty(Street);
        public bool ShouldSerializeCity() => !string.IsNullOrEmpty(City);
        public bool ShouldSerializeProvince() => !string.IsNullOrEmpty(Province);
        public bool ShouldSerializePostalCode() => !string.IsNullOrEmpty(PostalCode);
        public bool ShouldSerializeCountry() => !string.IsNullOrEmpty(Country);
        public bool ShouldSerializePhone() => !string.IsNullOrEmpty(Phone);
        public bool ShouldSerializeTollFree() => !string.IsNullOrEmpty(TollFree);
        public bool ShouldSerializeMobile() => !string.IsNullOrEmpty(Mobile);
        public bool ShouldSerializeFax() => !string.IsNullOrEmpty(Fax);
        public bool ShouldSerializeEmail() => !string.IsNullOrEmpty(Email);

        public override string ToString() =>
            LocationHelper.ToString(Street, null, City, Province, PostalCode, null, Phone, Fax);

        public bool IsEqual(Location other)
        {
            return LocationType == other.LocationType
                && Description.NullIfEmpty() == other.Description.NullIfEmpty()
                && TimeZone.NullIfEmpty() == other.TimeZone.NullIfEmpty()
                && Street.NullIfEmpty() == other.Street.NullIfEmpty()
                && City.NullIfEmpty() == other.City.NullIfEmpty()
                && Province.NullIfEmpty() == other.Province.NullIfEmpty()
                && PostalCode.NullIfEmpty() == other.PostalCode.NullIfEmpty()
                && Country.NullIfEmpty() == other.Country.NullIfEmpty()
                && Phone.NullIfEmpty() == other.Phone.NullIfEmpty()
                && TollFree.NullIfEmpty() == other.TollFree.NullIfEmpty()
                && Mobile.NullIfEmpty() == other.Mobile.NullIfEmpty()
                && Fax.NullIfEmpty() == other.Fax.NullIfEmpty()
                && Email.NullIfEmpty() == other.Email.NullIfEmpty();
        }
    }
}