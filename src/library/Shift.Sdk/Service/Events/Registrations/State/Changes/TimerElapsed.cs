using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class TimerElapsed : Change
    {
        public Guid Timer { get; set; }

        public TimerElapsed(Guid timer)
        {
            Timer = timer;
        }
    }
}
