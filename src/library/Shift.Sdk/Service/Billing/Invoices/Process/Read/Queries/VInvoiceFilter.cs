using System;

using Shift.Common;

namespace InSite.Application.Invoices.Read
{
    [Serializable]
    public class VInvoiceFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? CustomerIdentifier { get; set; }
        public Guid? ProductIdentifier { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string InvoiceStatus { get; set; }
        public string[] ExcludeInvoiceStatuses { get; set; }
        public DateTimeOffset? InvoiceDraftedSince { get; set; }
        public DateTimeOffset? InvoiceDraftedBefore { get; set; }
        public DateTimeOffset? InvoiceSubmittedSince { get; set; }
        public DateTimeOffset? InvoiceSubmittedBefore { get; set; }
        public DateTimeOffset? InvoicePaidSince { get; set; }
        public DateTimeOffset? InvoicePaidBefore { get; set; }
        public string CustomerEmployer { get; set; }
        public string CustomerPersonCode { get; set; }
        public int? InvoiceNumber { get; set; }
        public string TransactionIdentifier { get; set; }
    }
}
