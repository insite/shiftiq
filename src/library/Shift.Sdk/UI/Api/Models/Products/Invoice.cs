using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class Invoice
    {
        public Guid CustomerIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string InvoiceStatus { get; set; }
        public int? InvoiceNumber { get; set; }

        public DateTimeOffset? InvoiceDrafted { get; set; }
        public DateTimeOffset? InvoicePaid { get; set; }
        public DateTimeOffset? InvoiceSubmitted { get; set; }

        public List<InvoiceItem> InvoiceItems { get; set; }
    }
}