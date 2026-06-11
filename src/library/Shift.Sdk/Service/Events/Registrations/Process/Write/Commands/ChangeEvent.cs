using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeEvent : Command
    {
        public Guid Event { get; private set; }
        public string Reason { get; private set; }
        public bool CancelEmptyEvent { get; private set; }

        public ChangeEvent(Guid registration, Guid @event, string reason, bool cancelEmptyEvent)
        {
            AggregateIdentifier = registration;
            Event = @event;
            Reason = reason;
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}