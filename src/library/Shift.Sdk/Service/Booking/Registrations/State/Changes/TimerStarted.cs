using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class TimerStarted : Change
    {
        public Guid Timer { get; set; }
        public DateTimeOffset At { get; set; }
        public string Description { get; set; }

        public TimerStarted(Guid timer, DateTimeOffset at, string description)
        {
            Timer = timer;
            At = at;
            Description = description;
        }
    }
}
