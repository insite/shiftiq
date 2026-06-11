using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventRescheduled : Change
    {
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public EventRescheduled(DateTimeOffset start, DateTimeOffset end)
        {
            StartTime = start;
            EndTime = end;
        }
    }
}