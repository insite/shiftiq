using System;

namespace InSite.Application.Invoices.Read
{
    public class QInvoiceItem
    {
        public Guid InvoiceIdentifier { get; set; }
        public Guid ItemIdentifier { get; set; }
        public Guid ProductIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string ItemDescription { get; set; }

        public int ItemQuantity { get; set; }
        public int ItemSequence { get; set; }

        public decimal ItemPrice { get; set; }
        public decimal? TaxRate { get; set; }

        public virtual TProduct Product { get; set; }
        public virtual QInvoice Invoice { get; set; }
        public virtual VInvoice VInvoice { get; set; }
    }
}
