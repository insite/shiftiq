using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Events.Appointments
{
    public partial class Search : PortalBasePage
    {
        private class EventItem
        {
            public Guid EventIdentifier { get; set; }
            public string EventTitle { get; set; }
            public string AppointmentType { get; set; }
            public DateTimeOffset EventScheduledStart { get; set; }
            public DateTimeOffset? EventScheduledEnd { get; set; }
            public string VenueLocationName { get; set; }
            public string VenueAddress { get; set; }
            public string Summary { get; set; }

            public string EventScheduledText
            {
                get
                {
                    return EventScheduledEnd == null || EventScheduledEnd.Value.Date == EventScheduledStart.Date
                        ? $"{EventScheduledStart.Format(User.TimeZone, true)}"
                        : $"{EventScheduledStart.Format(User.TimeZone, true)} to {EventScheduledEnd.Format(User.TimeZone, true)}";
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this);

            var appointments = GetAppointments();

            MainPanel.Visible = appointments.Count > 0;

            if (appointments.Count > 0)
            {
                EventRepeater.DataSource = appointments;
                EventRepeater.DataBind();
            }
            else
            {
                StatusAlert.AddMessage(AlertType.Warning, Translate("There are no published classes"));
            }
        }

        private List<EventItem> GetAppointments()
        {
            var events = GetEvents();

            var appointments = new List<EventItem>();
            foreach (var ev in events)
            {
                var item = new EventItem
                {
                    EventIdentifier = ev.EventIdentifier,
                    EventTitle = ev.EventTitle,
                    AppointmentType = ev.AppointmentType,
                    EventScheduledStart = ev.EventScheduledStart,
                    EventScheduledEnd = ev.EventScheduledEnd,
                    VenueLocationName = ev.VenueLocationName,
                    VenueAddress = GetVenueAddress(ev),
                    Summary = GetDescription(ev)
                };

                appointments.Add(item);
            }

            return appointments;
        }

        private List<QEvent> GetEvents()
        {
            var filterByStart = new QEventFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                EventType = "Appointment",
                EventScheduleEndSince = DateTime.UtcNow,
                EventPublicationStatus = PublicationStatus.Published.GetDescription()
            };

            var events = ServiceLocator.EventSearch
                .GetEvents(filterByStart, null, null)
                .Where(x => x.EventSchedulingStatus != "Cancelled")
                .ToList();

            var result = new List<QEvent>(events);

            return result;
        }

        #region Helper methods

        private static string GetDescription(QEvent @event)
        {
            return Markdown.ToHtml(ContentEventClass.Deserialize(@event.Content).Description.Default);
        }

        private static string GetVenueAddress(QEvent @event)
        {
            if (@event.VenueLocationIdentifier == null)
                return String.Empty;

            var address = ServiceLocator.GroupSearch.GetAddress(@event.VenueLocationIdentifier.Value, AddressType.Physical);

            if (address == null)
                return string.Empty;

            if (address.City.HasValue() && address.Province.HasValue())
                return $"{address.City}, {address.Province}";

            if (address.City.HasValue())
                return address.City;

            return address.Province;
        }
        #endregion
    }
}