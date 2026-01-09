using System;
using System.Collections.Generic;

using InSite.Application.Payments.Read;

namespace InSite.Application.Invoices.Read
{
    public class VInvoice
    {
        public Guid CustomerIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public Guid? BusinessCustomerGroupIdentifier { get; set; }
        public string BusinessCustomerName { get; set; }
        public Guid? EmployeeUserIdentifier { get; set; }
        public string EmployeeName { get; set; }
        public Guid? IssueIdentifier { get; set; }
        public string IssueTitle { get; set; }

        public Guid? ReferencedInvoiceIdentifier { get; set; }

        public int? InvoiceNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        public string InvoiceStatus { get; set; }

        public int? ItemCount { get; set; }

        public decimal? InvoiceAmount { get; set; }

        public DateTimeOffset? InvoiceDrafted { get; set; }
        public DateTimeOffset? InvoicePaid { get; set; }
        public DateTimeOffset? InvoiceSubmitted { get; set; }

        public virtual ICollection<QPayment> Payments { get; set; } = new HashSet<QPayment>();
        public virtual ICollection<QInvoiceItem> InvoiceItems { get; set; } = new HashSet<QInvoiceItem>();

        public string CustomerEmployer { get; set; }
        public string CustomerPersonCode { get; set; }
    }
}