using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Constant;

namespace InSite.Admin.Events.Appointments.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QEventFilter>
    {
        public override QEventFilter Filter
        {
            get
            {
                var filter = new QEventFilter
                {
                    EventType = EventType.Appointment.ToString(),
                    OrganizationIdentifier = Organization.Identifier,

                    EventScheduledSince = EventScheduledSince.Value,
                    EventScheduledBefore = EventScheduledBefore.Value,
                    EventTitle = EventTitle.Text,
                    AppointmentType = AppointmentType.Value,
                    EventDescription = EventDescription.Text
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                EventScheduledSince.Value = value.EventScheduledSince;
                EventScheduledBefore.Value = value.EventScheduledBefore;
                EventTitle.Text = value.EventTitle;
                AppointmentType.Value = value.AppointmentType;
                EventDescription.Text = value.EventDescription;
            }
        }

        public override void Clear()
        {
            EventScheduledSince.Value = null;
            EventScheduledBefore.Value = null;
            EventTitle.Text = null;
            AppointmentType.Value = null;
            EventDescription.Text = null;
        }
    }
}