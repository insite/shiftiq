using Shift.Common;

namespace InSite.Domain.Contacts
{
    public class GroupAddress
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public GroupAddress Clone(bool nullIfEmpty = false)
        {
            return new GroupAddress
            {
                City = City.NullIfEmpty(),
                Country = Country.NullIfEmpty(),
                Description = Description.NullIfEmpty(),
                PostalCode = PostalCode.NullIfEmpty(),
                Province = Province.NullIfEmpty(),
                Street1 = Street1.NullIfEmpty(),
                Street2 = Street2.NullIfEmpty(),
                Latitude = Latitude,
                Longitude = Longitude,
            };
        }
    }
}
