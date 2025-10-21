using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class Venue : BaseUserControl
    {
        public void BindVenue(QEvent @event, string venueAddressLabel, string venueLabel)
        {
            VenueLabel.InnerText = venueLabel;
            if (@event.VenueLocation == null)
            {
                VenueName.Text = Translate("None");
                VenueAddress.Visible = false;
            }
            else
            {
                VenueName.Text = @event.VenueLocation.GroupName;

                var address = ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical);

                VenueAddress.BindAddress(address, venueAddressLabel);
            }
        }
    }
}