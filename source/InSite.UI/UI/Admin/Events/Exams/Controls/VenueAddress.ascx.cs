using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class VenueAddress : UserControl
    {
        public void BindAddress(QGroupAddress address, string venueAddressLabel, string helpBlockText)
        {
            var text = address != null
                ? GetAddress(address)
                : string.Empty;

            BindAddress(text, venueAddressLabel, helpBlockText);
        }

        public void BindAddress(Address address, string venueAddressLabel, string helpBlockText)
        {
            var text = address != null
                ? GetAddress(address)
                : string.Empty;

            BindAddress(text, venueAddressLabel, helpBlockText);
        }

        public void BindAddress(Guid? addressIdentifier, string venueAddressLabel, string helpBlockText)
        {
            var text = addressIdentifier != null
                ? GetAddress(ServiceLocator.ContactSearch.GetAddress(addressIdentifier.Value))
                : string.Empty;

            BindAddress(text, venueAddressLabel, helpBlockText);
        }

        private void BindAddress(string addressText, string venueAddressLabel, string helpBlockText)
        {
            VenueDetails.Text = addressText.HasValue()
                ? addressText
                : "<span class='text-danger'>None</span>";

            VenueAddressLabel.InnerText = venueAddressLabel;
            HelpBlock.InnerText = helpBlockText;
            HelpBlock.Visible = !(helpBlockText == string.Empty);
        }

        private string GetAddress(QGroupAddress address)
        {
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        private string GetAddress(Address address)
        {
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }

        private string GetAddress(VAddress address)
        {
            return LocationHelper.ToHtml(address.Street1, address.Street2, address.City, address.Province, address.PostalCode, null, null, null);
        }
    }
}