using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class SeatDeleted : Change
    {
        public Guid Seat { get; set; }

        public SeatDeleted(Guid seat)
        {
            Seat = seat;
        }
    }
}
