using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceItemAdded : Change
    {
        public InvoiceItemAdded(InvoiceItem item)
        {
            Item = item;
        }

        public InvoiceItem Item { get; set; }
    }
}
