using System.Web.UI;

using InSite.Application.Events.Read;

using Shift.Common;

namespace InSite.Admin.Events.Appointments.Controls
{
    public partial class AppointmentInfo : UserControl
    {
        public void BindAppointment(QEvent ev, bool showSchedule = true)
        {
            EventTitle.Text = $"<a href=\"/ui/admin/events/appointments/outline?event={ev.EventIdentifier}\">{ev.EventTitle}</a>";
            AppointmentType.Text = ev.AppointmentType.IfNullOrEmpty("N/A");
            EventScheduledStart.Text = ev.EventScheduledStart.Format(null, true);
            EventScheduledEnd.Text = ev.EventScheduledEnd.Format(null, true, nullValue: "None");

            EventScheduledStartLabel.Visible =
            EventScheduledStartValue.Visible =
            EventScheduledEndLabel.Visible =
            EventScheduledEndValue.Visible = showSchedule;
        }
    }
}