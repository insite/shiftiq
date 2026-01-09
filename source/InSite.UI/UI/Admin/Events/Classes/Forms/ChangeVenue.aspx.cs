using System;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class ChangeVenue : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventIdentifier =>
            Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier.Value}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Open()
        {
            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation)
                : null;

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            VenueGroupIdentifier.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
            VenueGroupIdentifier.Filter.GroupType = GroupTypes.Venue;

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            // 1 - Physical Address
            VenueInfo.Bind(@event.EventIdentifier, @event.VenueLocation, AddressType.Physical);
            CurrentVenueRoomField.Visible = @event.VenueRoom.HasValue();
            CurrentVenueRoom.Text = @event.VenueRoom;

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var venueId = VenueGroupIdentifier.Value;

            ServiceLocator.SendCommand(new ChangeEventVenue(EventIdentifier.Value, venueId, venueId, NewVenueRoom.Text));

            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}