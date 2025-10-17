using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class TimerCancelled : Change
    {
        public Guid Timer { get; set; }

        public TimerCancelled(Guid timer)
        {
            Timer = timer;
        }
    }
}
