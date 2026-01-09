using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class TakeAttendance : Command
    {
        public string Status { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }

        public TakeAttendance(Guid aggregate, string status, decimal? quantity, string unit)
        {
            AggregateIdentifier = aggregate;
            
            Status = status;
            Quantity = quantity;
            Unit = unit;
        }
    }
}
