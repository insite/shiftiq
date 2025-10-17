using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Sales.Invoices.Changes
{
    public class InvoiceBusinessCustomerChanged : Change
    {
        public InvoiceBusinessCustomerChanged(Guid? businessCustomer)
        {
            BusinessCustomer = businessCustomer;
        }

        public Guid? BusinessCustomer { get; set; }
    }
}
