using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class MeetingScheduled : Change
    {
        public Guid Tenant { get; set; }

        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public int Duration { get; set; }
        public int Number { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public MeetingScheduled(Guid tenant, string title, string status, string description, int duration, int number, DateTimeOffset start)
        {
            Tenant = tenant;

            Title = title;
            Status = status;
            Description = description;

            Duration = duration;
            Number = number;

            StartTime = start;
        }
    }

    public class MeetingScheduled2 : Change
    {
        public Guid Tenant { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public int Duration { get; set; }
        public int Number { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public MeetingScheduled2(Guid tenant, string title, string status, string description, int duration, int number, DateTimeOffset start)
        {
            Tenant = tenant;
            Title = title;
            Status = status;
            Description = description;

            Duration = duration;
            Number = number;

            StartTime = start;
        }
    }
}