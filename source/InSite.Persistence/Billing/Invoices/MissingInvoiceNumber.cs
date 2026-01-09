using System;

namespace InSite.Persistence
{
    public class MissingInvoiceNumber
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid InvoiceIdentifier { get; set; }
        public DateTimeOffset InvoiceDrafted { get; set; }
    }
}
