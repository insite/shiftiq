using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class AppointmentTypeChanged : Change
    {
        public string AppointmentType { get; }

        public AppointmentTypeChanged(string appointmentType)
        {
            AppointmentType = appointmentType;
        }
    }
}
