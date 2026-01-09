using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoiceStatusChanged : Change
    {
        public InvoiceStatusChanged(String status)
        {
            InvoiceStatus = status;
        }

        public string InvoiceStatus { get; set; }
    }
}
