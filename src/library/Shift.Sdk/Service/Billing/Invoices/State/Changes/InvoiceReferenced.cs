using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceReferenced : Change
    {
        public Guid? ReferencedInvoice { get; }

        public InvoiceReferenced(Guid? referencedInvoiced)
        {
            ReferencedInvoice = referencedInvoiced;
        }
    }
}
