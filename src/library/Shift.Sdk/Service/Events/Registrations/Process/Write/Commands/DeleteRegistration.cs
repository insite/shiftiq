using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class DeleteRegistration : Command
    {
        public bool CancelEmptyEvent { get; }

        public DeleteRegistration(Guid aggregate, bool cancelEmptyEvent)
        {
            AggregateIdentifier = aggregate;
            CancelEmptyEvent = cancelEmptyEvent;
        }
    }
}