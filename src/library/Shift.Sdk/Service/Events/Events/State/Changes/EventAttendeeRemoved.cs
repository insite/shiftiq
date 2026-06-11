using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventAttendeeRemoved : Change
    {
        public Guid Contact { get; set; }
        public string Role { get; set; }

        public EventAttendeeRemoved(Guid contact, string role)
        {
            Contact = contact;
            Role = role;
        }
    }
}
