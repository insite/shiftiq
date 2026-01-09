using System;

using InSite.Application.Events.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;

namespace InSite.UI.Portal.Events.Appointments
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
                HttpResponseHelper.Redirect("/ui/portal/events/appointments/search");
                return;
            }
            SetInputValues(@event);
        }

        private void SetInputValues(QEvent @event)
        {
            PageHelper.AutoBindHeader(this);

            var scheduledDateAndTime = GetEventDateAndTimeText(@event.EventScheduledStart, @event.EventScheduledEnd);

            PageHelper.OverrideTitle(this, @event.EventTitle, @event.AppointmentType);

            var content = ContentEventClass.Deserialize(@event.Content);

            var description = content.Description != null && content.Description.Default.HasValue() ? Markdown.ToHtml(content.Description.Default) : string.Empty;
            DescriptionPanel.Visible = !string.IsNullOrEmpty(description);
            Description.Text = description;

            Date.Text = scheduledDateAndTime;

            var localeDate = TimeZoneInfo.ConvertTime(@event.EventScheduledStart, User.TimeZone).Date;
            ReturnToCalendar.NavigateUrl = $"/ui/portal/events/calendar?date={localeDate.ToShortDateString()}";
            ReturnToCalendar.Visible = !(Identity.Organization.Toolkits.Events?.HideReturnToCalendar ?? false);
        }

        private string GetEventDateAndTimeText(DateTimeOffset start, DateTimeOffset? end)
        {
            if (end == null)
                return $"{start.Format(User.TimeZone, true)}";

            return start.Date != end.Value.Date
                ? $"{start.Format(User.TimeZone, true)} - {end.Value.Format(User.TimeZone, true)}"
                : $"{start.Format(User.TimeZone, true)} - {end.Value.FormatTimeOnly(User.TimeZone)}";
        }
    }
}