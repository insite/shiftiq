using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ScheduleAppointment : Command
    {
        public Guid Tenant { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public string Title { get; set; }
        public string AppointmentType { get; set; }
        public string Description { get; set; }

        public ScheduleAppointment(Guid @event, Guid tenant, string title, string appointmentType, string description, DateTimeOffset start, DateTimeOffset end)
        {
            AggregateIdentifier = @event;

            Tenant = tenant;
            Title = title;
            AppointmentType = appointmentType;
            Description = description;

            StartTime = start;
            EndTime = end;
        }
    }
}
