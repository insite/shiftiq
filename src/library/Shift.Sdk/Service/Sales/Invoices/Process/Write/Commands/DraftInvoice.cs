using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Invoices;

namespace InSite.Application.Invoices.Write
{
    public class DraftInvoice : Command
    {
        public DraftInvoice(Guid aggregate, Guid tenant, int number, Guid customer, InvoiceItem[] items)
        {
            AggregateIdentifier = aggregate;
            Tenant = tenant;
            Number = number;
            Customer = customer;
            Items = items;
        }

        public Guid Tenant { get; set; }
        public int Number { get; set; }
        public Guid Customer { get; set; }
        public InvoiceItem[] Items { get; set; }
    }
}