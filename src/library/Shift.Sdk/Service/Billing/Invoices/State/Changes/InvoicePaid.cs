using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Invoices
{
    public class InvoicePaid : Change
    {
        public DateTimeOffset? Paid { get; }

        public InvoicePaid(DateTimeOffset? paid)
        {
            Paid = paid;
        }
    }
}