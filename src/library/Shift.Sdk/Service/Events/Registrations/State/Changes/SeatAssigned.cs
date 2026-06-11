using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class SeatAssigned : Change
    {
        public Guid? Seat { get; set; }
        public decimal? Price { get; set; }
        public string BillingCustomer { get; set; }

        public SeatAssigned(Guid? seat, decimal? price, string billingCustomer)
        {
            Seat = seat;
            Price = price;
            BillingCustomer = billingCustomer;
        }
    }
}
