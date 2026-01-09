using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassVenueAddressInfo : UserControl
    {
        public void Bind(QGroupAddress address)
        {
            var text = string.Empty;
            var link = string.Empty;

            if (address != null)
            {
                text = GetAddress(address);
                link = address.GetGMapsAddressLink();
            }

            Bind(text, link);
        }

        public void Bind(Guid? addressIdentifier)
        {
            var address = addressIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetAddress(addressIdentifier.Value)
                : null;

            var text = string.Empty;
            var link = string.Empty;

            if (address != null)
            {
                text = GetAddress(address);
                link = address.GetGMapsAddressLink();
            }

            Bind(text, link);
        }

        private void Bind(string text, string link)
        {
            FieldHtml.InnerHtml = text.IsEmpty()
                ? "<span class='text-danger'>None</span>"
                : link.IsNotEmpty()
                    ? $"<a href='{link}' target='_blank'>{text}</a>"
                    : text;
        }

        public static string GetAddress(Guid? venueIdentifier, AddressType type)
        {
            var address = venueIdentifier.HasValue
                ? ServiceLocator.GroupSearch.GetAddress(venueIdentifier.Value, type)
                : null;

            return GetAddress(address);
        }

        public static string GetAddress(QGroupAddress address)
        {
            return address == null
                ? string.Empty
                : LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(QPersonAddress address)
        {
            return address == null
                ? string.Empty
                : LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(Address address)
        {
            return address == null
                ? string.Empty
                : LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(VAddress address)
        {
            return address == null
                ? string.Empty
                : LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }
    }
}