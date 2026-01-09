using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class RemoveEventAttendee : Command
    {
        public Guid ContactIdentifier { get; set; }

        public RemoveEventAttendee(Guid @event, Guid contact)
        {
            AggregateIdentifier = @event;
            ContactIdentifier = contact;
        }
    }
}
