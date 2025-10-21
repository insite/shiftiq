using System;
using System.Web.UI.WebControls;

using InSite.Application.Events.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Events.Classes.Forms
{
    public partial class Reschedule : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/events/classes/search";

        private Guid? EventIdentifier =>
            Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/classes/outline?event={EventIdentifier.Value}&panel=class";

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"event={EventIdentifier}&panel=class" : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => OnConfirmed();

            EventScheduledEndValidator.ServerValidate += EventScheduledEndValidator_ServerValidate;

            // Aleksey 2022-02-10: auto postbacks prevent the form to be saved time to time.
            // The solution needs to be re-worked.
            //
            //EventScheduledStart.AutoPostBack = true;
            //EventScheduledStart.ValueChanged += EventDate_ValueChanged;
            //EventScheduledEnd.AutoPostBack = true;
            //EventScheduledEnd.ValueChanged += EventDate_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanEdit)
                HttpResponseHelper.Redirect(SearchUrl);

            Open();
        }

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation)
                : null;

            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled to start {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            SummaryInfo.Bind(@event);
            LocationInfo.Bind(@event);

            EventScheduledStart.Value = @event.EventScheduledStart;
            EventScheduledEnd.Value = @event.EventScheduledEnd;
            Duration.ValueAsInt = @event.DurationQuantity;
            DurationUnit.Value = @event.DurationUnit;
            CreditHours.ValueAsDecimal = @event.CreditHours;

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void OnConfirmed()
        {
            if (!Page.IsValid)
                return;

            var @event = ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation);

            if (@event.EventScheduledStart != EventScheduledStart.Value || @event.EventScheduledEnd != EventScheduledEnd.Value)
                ServiceLocator.SendCommand(new RescheduleEvent(@event.EventIdentifier, EventScheduledStart.Value.Value, EventScheduledEnd.Value.Value));

            if (@event.DurationQuantity != Duration.ValueAsInt || @event.DurationUnit != DurationUnit.Value)
                ServiceLocator.SendCommand(new ChangeEventDuration(@event.EventIdentifier, Duration.ValueAsInt.Value, DurationUnit.Value));

            if (@event.CreditHours != CreditHours.ValueAsDecimal)
                ServiceLocator.SendCommand(new ChangeEventCreditHours(@event.EventIdentifier, CreditHours.ValueAsDecimal));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private void EventDate_ValueChanged(object sender, EventArgs e)
        {
            if (DurationUnit.Value == "Day")
            {
                if (EventScheduledEnd.Value.HasValue && EventScheduledStart.Value.HasValue)
                {
                    var days = (int)EventScheduledEnd.Value.Value.DateTime.Date.Subtract(EventScheduledStart.Value.Value.DateTime.Date).TotalDays + 1;
                    if (days > 0)
                        Duration.ValueAsInt = days;
                }
            }
        }

        private void EventScheduledEndValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (EventScheduledEnd.Value?.UtcDateTime <= EventScheduledStart.Value?.UtcDateTime)
            {
                args.IsValid = false;
                EventScheduledEndValidator.ErrorMessage = "End must be later than Start";
            }
        }
    }
}