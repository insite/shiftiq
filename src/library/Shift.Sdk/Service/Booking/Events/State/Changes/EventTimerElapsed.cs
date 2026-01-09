using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventTimerElapsed : Change
    {
        public Guid Timer { get; set; }

        public EventTimerElapsed(Guid timer)
        {
            Timer = timer;
        }
    }
}