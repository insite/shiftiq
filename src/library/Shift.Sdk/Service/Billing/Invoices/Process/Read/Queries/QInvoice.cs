using System;
using System.Collections.Generic;

namespace InSite.Application.Invoices.Read
{
    public class QInvoice
    {
        public Guid CustomerIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public Guid? BusinessCustomerGroupIdentifier { get; set; }
        public Guid? EmployeeUserIdentifier { get; set; }
        public Guid? IssueIdentifier { get; set; }

        public Guid? ReferencedInvoiceIdentifier { get; set; }
        public string InvoiceStatus { get; set; }

        public int? InvoiceNumber { get; set; }

        public DateTimeOffset? InvoiceDrafted { get; set; }
        public DateTimeOffset? InvoicePaid { get; set; }
        public DateTimeOffset? InvoiceSubmitted { get; set; }

        public virtual ICollection<QInvoiceItem> InvoiceItems { get; set; } = new HashSet<QInvoiceItem>();
        public virtual ICollection<TOrder> Orders { get; set; } = new HashSet<TOrder>();
    }
}
