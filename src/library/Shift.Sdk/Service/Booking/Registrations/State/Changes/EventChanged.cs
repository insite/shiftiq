using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class EventChanged : Change
    {
        public Guid Event { get; private set; }
        public string Reason { get; private set; }
        public bool CancelEmptyEvent { get; private set; }

        public EventChanged(Guid @event, string reason, bool cancelEmptyEvent)
        {
            Event = @event;
            Reason = reason;
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}
