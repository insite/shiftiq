using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventRegistrationLocked : Change
    {
        public DateTimeOffset Locked { get; set; }

        public EventRegistrationLocked(DateTimeOffset locked)
        {
            Locked = locked;
        }
    }
}
