using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Web.Helpers
{
    public static class AddressExtensions
    {
        public static string GetGMapsAddressLink(this QGroupAddress address)
            => address is null
            ? string.Empty
            : LocationHelper.GetGMapsAddressLink(address.Street1, address.Street2, address.City, address.Province, address.Country, address.PostalCode);

        public static string GetGMapsAddressLink(this QPersonAddress address)
            => address is null
            ? string.Empty
            : LocationHelper.GetGMapsAddressLink(address.Street1, address.Street2, address.City, address.Province, address.Country, address.PostalCode);

        public static string GetGMapsAddressLink(this VAddress address)
            => address is null
            ? string.Empty
            : LocationHelper.GetGMapsAddressLink(address.Street1, address.Street2, address.City, address.Province, address.Country, address.PostalCode);
    }
}