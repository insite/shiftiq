using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoicePaidDateChanged : Change
    {
        public InvoicePaidDateChanged(DateTimeOffset? dateTimeOffset)
        {
            InvocePaidDate = dateTimeOffset;
        }

        public DateTimeOffset? InvocePaidDate { get; set; }
    }
}
