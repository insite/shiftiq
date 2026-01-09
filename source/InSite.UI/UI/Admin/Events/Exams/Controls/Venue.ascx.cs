using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;

using Shift.Constant;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class Venue : UserControl
    {
        public void BindVenue(QEvent @event, VGroup venue, AddressType typeOfAddress, string venueAddressLabel, string helpBlockText, string venueLabel, bool showChangeVenue)
        {
            VenueLabel.InnerText = venueLabel;
            if (venue == null)
            {
                VenueName.Text = "None";
                VenueAddress.Visible = false;
            }
            else
            {
                VenueName.Text = $"<a href=\"/ui/admin/contacts/groups/edit?contact={venue.GroupIdentifier}\">{venue.GroupName}</a>";
                if (typeOfAddress == AddressType.Physical)
                {
                    var address = ServiceLocator.GroupSearch.GetAddress(venue.GroupIdentifier, AddressType.Physical);

                    VenueAddress.BindAddress(address, venueAddressLabel, helpBlockText);
                }
            }

            ExamChangeVenue.Visible = showChangeVenue;
            ExamChangeVenue.NavigateUrl = $"/ui/admin/events/exams/change-venue?event={@event.EventIdentifier}";
        }

    }
}