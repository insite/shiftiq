using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Invoices;

namespace InSite.Application.Invoices.Write
{
    public class AddInvoiceItem : Command
    {
        public AddInvoiceItem(Guid invoice, InvoiceItem item)
        {
            AggregateIdentifier = invoice;
            Item = item;
        }

        public InvoiceItem Item { get; set; }
    }
}
