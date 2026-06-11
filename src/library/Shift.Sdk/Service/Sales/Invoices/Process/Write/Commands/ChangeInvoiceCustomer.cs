using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class ChangeInvoiceCustomer : Command
    {
        public ChangeInvoiceCustomer(Guid aggregate, Guid customer)
        {
            AggregateIdentifier = aggregate;
            Customer = customer;
        }

        public Guid Customer { get; set; }
    }
}
