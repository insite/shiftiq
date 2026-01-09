using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceCustomerChanged : Change
    {
        public InvoiceCustomerChanged(Guid customer)
        {
            Customer = customer;
        }

        public Guid Customer { get; set; }
    }
}
