using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class DeleteSeat : Command
    {
        public Guid Seat { get; }
        public string Reason { get; }

        public DeleteSeat(Guid @event, Guid seat)
        {
            AggregateIdentifier = @event;
            Seat = seat;
        }
    }
}
