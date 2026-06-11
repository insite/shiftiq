using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class PayInvoice : Command
    {
        public DateTimeOffset? Paid { get; }

        public PayInvoice(Guid aggregate, DateTimeOffset? paid)
        {
            AggregateIdentifier = aggregate;
            Paid = paid;
        }
    }
}