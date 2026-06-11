using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class AddEventAttendee : Command
    {
        public Guid ContactIdentifier { get; set; }
        public string ContactRole { get; set; }
        public bool Validate { get; set; }

        public AddEventAttendee(Guid @event, Guid contact, string role, bool validate)
        {
            AggregateIdentifier = @event;
            ContactIdentifier = contact;
            ContactRole = role;
            Validate = validate;
        }
    }
}
