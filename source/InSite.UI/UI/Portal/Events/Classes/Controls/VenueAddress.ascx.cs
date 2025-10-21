using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class VenueAddress : UserControl
    {
        public void BindAddress(QGroupAddress address, string venueAddressLabel)
        {
            var text = address != null
                ? GetAddress(address)
                : string.Empty;

            BindAddress(text, address?.GetGMapsAddressLink(), venueAddressLabel);
        }

        public void BindAddress(Guid? addressIdentifier, string venueAddressLabel)
        {
            var address = addressIdentifier.HasValue
                ? ServiceLocator.ContactSearch.GetAddress(addressIdentifier.Value)
                : null;

            var text = address != null
                ? GetAddress(address)
                : string.Empty;

            BindAddress(text, address?.GetGMapsAddressLink(), venueAddressLabel);
        }

        private void BindAddress(string addressText, string addressLink, string venueAddressLabel)
        {
            VenueDetails.Text = addressText.HasValue()
                ? addressLink.HasValue()
                    ? $"<a href='{addressLink}' target='_blank'>{addressText}</a>"
                    : addressText
                : $"<span class='text-danger'>{Common.LabelHelper.GetTranslation("None")}</span>";

            VenueAddressLabel.InnerText = venueAddressLabel;
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
            if (address == null) 
                return string.Empty;
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        public static string GetAddress(VAddress address)
        {
            if (address == null)
                return string.Empty;
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }
    }
}