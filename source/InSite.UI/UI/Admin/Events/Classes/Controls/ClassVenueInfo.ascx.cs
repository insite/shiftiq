using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;

using Shift.Constant;

namespace InSite.UI.Admin.Events.Classes.Controls
{
    public partial class ClassVenueInfo : UserControl
    {
        public string VenueLabel
        {
            get => FieldLabel.InnerText;
            set => FieldLabel.InnerText = value;
        }

        public bool ShowChangeLink
        {
            get => ChangeLink.Visible;
            set => ChangeLink.Visible = value;
        }

        public void Bind(Guid eventIdentifier, VGroup venue, AddressType addressType = AddressType.Physical)
        {
            ChangeLink.NavigateUrl = $"/ui/admin/events/classes/change-venue?event={eventIdentifier}";

            if (venue == null)
            {
                VenueName.InnerHtml = "None";
                Address.Visible = false;
                return;
            }

            VenueName.InnerHtml = $"<a href=\"/ui/admin/contacts/groups/edit?contact={venue.GroupIdentifier}\">{venue.GroupName}</a>";

            var address = ServiceLocator.GroupSearch.GetAddress(venue.GroupIdentifier, addressType);

            Address.Bind(address);
        }
    }
}