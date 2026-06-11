using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class CancelRegistration : Command
    {
        public string Reason { get; }
        public bool CancelEmptyEvent { get; }

        public CancelRegistration(Guid aggregate, string reason, bool cancelEmptyEvent)
        {
            AggregateIdentifier = aggregate;
            Reason = reason;
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}
