using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    [Obsolete]
    public class ClassScheduled : Change
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

        public ClassScheduled(Guid tenant, string title, string status, int number, DateTimeOffset start, DateTimeOffset end, int duration, string durationUnit, decimal? credit)
        {
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

    public class ClassScheduled2 : Change
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

        public ClassScheduled2(Guid tenant, string title, string status, int number, DateTimeOffset start, DateTimeOffset end, int duration, string durationUnit, decimal? credit)
        {
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