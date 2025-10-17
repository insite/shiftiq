using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Invoices.Read
{
    public class TOrder
    {
        public Guid OrderIdentifier { get; set; }
        public Guid InvoiceItemIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid CustomerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid ProductIdentifier { get; set; }
        public Guid ManagerUserIdentifier { get; set; }

        public string ProductUrl { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTimeOffset OrderCompleted { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public virtual QInvoice Invoice { get; set; }
        public virtual VUser Customer { get; set; }
        public virtual VUser Manager { get; set; }

        public virtual ICollection<TOrderItem> OrderItems { get; set; } = new HashSet<TOrderItem>();
    }
}
