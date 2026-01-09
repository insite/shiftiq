using System;

using Shift.Common.Timeline.Commands;
namespace InSite.Application.Invoices.Write
{
    public class ChangeInvoiceStatus : Command
    {
        public ChangeInvoiceStatus(Guid aggregate, string invoiceStatus)
        {
            AggregateIdentifier = aggregate;
            InvoiceStatus = invoiceStatus;
        }

        public string InvoiceStatus { get; set; }
    }
}
