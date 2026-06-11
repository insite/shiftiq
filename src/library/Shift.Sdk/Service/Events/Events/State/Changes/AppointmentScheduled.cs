using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class AppointmentScheduled : Change
    {
        public Guid Tenant { get; set; }
        public string Title { get; set; }
        public string AppointmentType { get; set; }
        public string Description { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public AppointmentScheduled(Guid tenant, string title, string appointmentType, string description, DateTimeOffset start, DateTimeOffset end)
        {
            Tenant = tenant;
            Title = title;
            AppointmentType = appointmentType;
            Description = description;

            StartTime = start;
            EndTime = end;
        }
    }
}
