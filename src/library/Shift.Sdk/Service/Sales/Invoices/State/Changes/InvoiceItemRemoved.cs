using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceItemRemoved : Change
    {
        public InvoiceItemRemoved(Guid item)
        {
            ItemIdentifier = item;
        }

        public Guid ItemIdentifier { get; set; }
    }
}
