using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceDrafted : Change
    {
        public InvoiceDrafted(Guid tenant, int number, Guid customer, InvoiceItem[] items)
        {
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

    public class InvoiceNumberChanged : Change
    {
        public InvoiceNumberChanged(int number)
        {
            Number = number;
        }

        public int Number { get; set; }
    }
}