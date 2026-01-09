using System;
using System.Globalization;

using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Events.Exams
{
    public partial class Outline : PortalBasePage
    {
        private Guid? EventID => Guid.TryParse(Request["event"], out var eventID) ? eventID : (Guid?)null;

        protected bool ShowFirstPriceColumn { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }


        private void LoadData()
        {
            var @event = EventID.HasValue ? ServiceLocator.EventSearch.GetEvent(EventID.Value, x => x.VenueLocation, x => x.Registrations) : null;

            if (@event == null || @event.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier)
            {
                HttpResponseHelper.Redirect("/ui/portal/events/exams/search");
                return;
            }
            SetInputValues(@event);
        }

        private void SetInputValues(QEvent @event)
        {
            PageHelper.AutoBindHeader(this);

            var content = ContentEventClass.Deserialize(@event.Content);
            var title = (content.Title?[Identity.Language]).IfNullOrEmpty(@event.EventTitle);

            var culture = CultureInfo.GetCultureInfo(Identity.Language);
            var scheduledDate = @event.EventScheduledEnd.HasValue && @event.EventScheduledEnd.Value.Date != @event.EventScheduledStart.Date
                ? $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone, culture)} - {@event.EventScheduledEnd.Value.FormatDateOnly(User.TimeZone, culture)}"
                : $"{@event.EventScheduledStart.FormatDateOnly(User.TimeZone, culture)}";

            PortalMaster.Breadcrumbs.BindTitleAndSubtitleNoTranslate(
                title,
                Translate("Scheduled") + " " + scheduledDate);

            Date.Text = GetEventDateAndTimeText(@event.EventScheduledStart, @event.EventScheduledEnd);

            var localeDate = TimeZoneInfo.ConvertTime(@event.EventScheduledStart, User.TimeZone).Date;
            ReturnToCalendar.NavigateUrl = $"/ui/portal/events/calendar?date={localeDate.ToShortDateString()}";
            ReturnToCalendar.Visible = !(Identity.Organization.Toolkits.Events?.HideReturnToCalendar ?? false);

            Venue.BindVenue(@event, Translate("Location"), Translate("Venue"));
        }

        private string GetEventDateAndTimeText(DateTimeOffset start, DateTimeOffset? end)
        {
            var culture = CultureInfo.GetCultureInfo(Identity.Language);

            if (end == null)
                return $"{start.Format(User.TimeZone, true, false, false, culture)}";

            return start.Date != end.Value.Date
                ? $"{start.Format(User.TimeZone, true, false, false, culture)} - {end.Value.Format(User.TimeZone, true, false, false, culture)}"
                : $"{start.Format(User.TimeZone, true, false, false, culture)} - {end.Value.FormatTimeOnly(User.TimeZone)}";
        }
    }
}