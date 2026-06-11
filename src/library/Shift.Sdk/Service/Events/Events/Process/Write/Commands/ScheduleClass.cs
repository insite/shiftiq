using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ScheduleClass : Command
    {
        public Guid Tenant { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }

        public int Number { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public int Duration { get; set; }
        public string DurationUnit { get; set; }
        public decimal? Credit { get; set; }

        public ScheduleClass(Guid @event, Guid tenant, string title, string status, int number, DateTimeOffset start, DateTimeOffset end, int duration, string durationUnit, decimal? credit)
        {
            AggregateIdentifier = @event;

            Tenant = tenant;
            Title = title;
            Status = status;

            Number = number;

            StartTime = start;
            EndTime = end;

            Duration = duration;
            DurationUnit = durationUnit;
            Credit = credit;
        }
    }
}
