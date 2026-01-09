using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventTimerCancelled : Change
    {
        public Guid Timer { get; set; }

        public EventTimerCancelled(Guid timer)
        {
            Timer = timer;
        }
    }
}