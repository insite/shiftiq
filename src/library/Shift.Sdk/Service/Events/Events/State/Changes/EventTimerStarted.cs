using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventTimerStarted : Change
    {
        public Guid Timer { get; set; }
        public DateTimeOffset At { get; set; }
        public string Description { get; set; }

        public EventTimerStarted(Guid timer, DateTimeOffset at, string description)
        {
            Timer = timer;
            At = at;
            Description = description;
        }
    }
}