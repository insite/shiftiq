using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class AssignSeat : Command
    {
        public Guid? Seat { get; set; }
        public decimal? Price { get; set; }
        public string BillingCustomer { get; set; }

        public AssignSeat(Guid aggregate, Guid? seat, decimal? price, string billingCustomer)
        {
            AggregateIdentifier = aggregate;
            Seat = seat;
            Price = price;
            BillingCustomer = billingCustomer;
        }
    }
}