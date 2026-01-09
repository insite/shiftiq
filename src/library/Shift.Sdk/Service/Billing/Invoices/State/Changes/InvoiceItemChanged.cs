using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceItemChanged : Change
    {
        public InvoiceItemChanged(InvoiceItem item)
        {
            Item = item;
        }

        public InvoiceItem Item { get; set; }
    }
}
