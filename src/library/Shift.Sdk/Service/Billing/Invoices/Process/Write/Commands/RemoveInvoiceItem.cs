using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class RemoveInvoiceItem : Command
    {
        public RemoveInvoiceItem(Guid invoice, Guid item)
        {
            AggregateIdentifier = invoice;
            ItemIdentifier = item;
        }

        public Guid ItemIdentifier { get; set; }
    }
}
