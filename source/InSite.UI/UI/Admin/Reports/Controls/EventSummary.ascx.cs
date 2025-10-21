using System;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Reports.Controls
{
    public partial class EventSummary : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DateType.AutoPostBack = true;
            DateType.ValueChanged += ApplyFilter_Click;

            DateRangeSelector.AutoPostBack = true;
            DateRangeSelector.ValueChanged += ApplyFilter_Click;

            AttendanceStatus.AutoPostBack = true;
            AttendanceStatus.ValueChanged += ApplyFilter_Click;

            EventFormat.AutoPostBack = true;
            EventFormat.ValueChanged += ApplyFilter_Click;

            ApplyFilter.Click += ApplyFilter_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DateRangeSelector.LoadItems(
                DateRangeShortcut.Today,
                DateRangeShortcut.Yesterday,
                DateRangeShortcut.ThisWeek,
                DateRangeShortcut.LastWeek,
                DateRangeShortcut.ThisMonth,
                DateRangeShortcut.LastMonth,
                DateRangeShortcut.ThisYear,
                DateRangeShortcut.LastYear);
            DateRangeSelector.Items.Add(new ComboBoxOption("Custom Dates", "Custom"));

            DateRangeSelector.Value = "Custom";
            DateRangeSince.Value = DateTime.Today.AddMonths(-3);
            DateRangeBefore.Value = DateTime.Today.AddMonths(3);

            LoadData();
        }

        private void ApplyFilter_Click(object sender, EventArgs e)
            => LoadData();

        private void LoadData()
        {
            try
            {
                var organization = CurrentSessionState.Identity.Organization.Identifier;

                var range = DateRangeSelector.Value == "Custom"
                    ? new DateTimeRange(DateRangeSince.Value, DateRangeBefore.Value)
                    : Calendar.GetDateTimeRange(DateRangeSelector.Value.ToEnum<DateRangeShortcut>(false));

                var eventFilter = new QEventFilter
                {
                    OrganizationIdentifier = organization,
                    EventFormat = EventFormat.Value,
                };

                var registrationFilter = new QRegistrationFilter
                {
                    OrganizationIdentifier = organization,
                    AttendanceStatus = AttendanceStatus.Value
                };

                if (DateType.Value == "Event Scheduled")
                {
                    eventFilter.EventScheduledSince = range.Since.ToDateTimeOffset(User.TimeZone);
                    eventFilter.EventScheduledBefore = range.Before.ToDateTimeOffset(User.TimeZone);

                    registrationFilter.EventScheduledSince = range.Since.ToDateTimeOffset(User.TimeZone);
                    registrationFilter.EventScheduledBefore = range.Before.ToDateTimeOffset(User.TimeZone);
                }
                else
                {
                    eventFilter.AttemptCompletedSince = range.Since.ToDateTimeOffset(User.TimeZone);
                    eventFilter.AttemptCompletedBefore = range.Before.ToDateTimeOffset(User.TimeZone);

                    registrationFilter.AttemptCompletedSince = range.Since.ToDateTimeOffset(User.TimeZone);
                    registrationFilter.AttemptCompletedBefore = range.Before.ToDateTimeOffset(User.TimeZone);
                }

                var events = ServiceLocator.EventSearch.GetEvents(eventFilter, null, null, x => x.VenueLocation);
                var registrations = ServiceLocator.RegistrationSearch.GetRegistrations(
                    registrationFilter,
                    x => x.Accommodations, x => x.Attempt, x => x.Event.VenueLocation, x => x.Form);

                var isAnalyticsHasData = AnalyticsTable.LoadData(events, registrations);
                var isRegistrationsHasData = RegistrationsTable.LoadData(registrations);
                var isAccommodationsHasData = AccommodationsTable.LoadData(registrations);

                NavigationTabs.Visible = isAnalyticsHasData || isRegistrationsHasData || isAccommodationsHasData;
                TabCounts.Visible = isAnalyticsHasData;
                TabRegistrations.Visible = isRegistrationsHasData;
                TabAccommodations.Visible = isAccommodationsHasData;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }
        }
    }
}