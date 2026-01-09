using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventAttendeeAdded : Change
    {
        public Guid Contact { get; set; }
        public string Role { get; set; }

        public EventAttendeeAdded(Guid contact, string role)
        {
            Contact = contact;
            Role = role;
        }
    }
}
