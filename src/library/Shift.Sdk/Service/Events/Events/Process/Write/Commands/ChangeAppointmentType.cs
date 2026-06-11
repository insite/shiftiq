using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeAppointmentType : Command
    {
        public string AppointmentType { get; }

        public ChangeAppointmentType(Guid @event, string appointmentType)
        {
            AggregateIdentifier = @event;
            AppointmentType = !string.IsNullOrEmpty(appointmentType) ? appointmentType : null;
        }
    }
}
