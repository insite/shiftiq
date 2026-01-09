using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class FailInvoicePayment : Command
    {
        public FailInvoicePayment(Guid invoice)
        {
            AggregateIdentifier = invoice;
        }
    }
}
