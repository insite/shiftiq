using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Controls
{
    public partial class VenueAddressOld : UserControl
    {
        public void BindAddress(QGroupAddress address, string venueAddressLabel, string helpBlockText)
        {
            var text = address != null
                ? GetAddress(address)
                : string.Empty;
            BindAddress(text, address.GetGMapsAddressLink(), venueAddressLabel, helpBlockText);
        }

        public void BindAddress(Guid? addressIdentifier, string venueAddressLabel, string helpBlockText)
        {
            var address = addressIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetAddress(addressIdentifier.Value)
                : null;

            var text = GetAddress(address);

            BindAddress(text, address.GetGMapsAddressLink(), venueAddressLabel, helpBlockText);
        }

        private void BindAddress(string addressText, string addressLink, string venueAddressLabel, string helpBlockText)
        {
            VenueDetails.Text = addressText.HasValue()
                ? addressLink.HasValue()
                    ? $"<a href='{addressLink}' target='_blank'>{addressText}</a>"
                    : addressText
                : "<span class='text-danger'>None</span>";

            VenueAddressLabel.InnerText = venueAddressLabel;
            HelpBlock.InnerText = helpBlockText;
            HelpBlock.Visible = !(helpBlockText == string.Empty);
        }

        public static string GetAddress(Guid? venueIdentifier, AddressType type)
        {
            if (venueIdentifier == null)
                return null;

            var address = ServiceLocator.GroupSearch.GetAddress(venueIdentifier.Value, type);

            return GetAddress(address);
        }

        public static string GetAddress(QGroupAddress address)
        {
            return address == null
                ? string.Empty
                : LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(Address address)
        {
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(VAddress address)
        {
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }
    }
}