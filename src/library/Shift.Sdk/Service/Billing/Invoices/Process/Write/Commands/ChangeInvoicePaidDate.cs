using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class ChangeInvoicePaidDate : Command
    {
        public ChangeInvoicePaidDate(Guid aggregate, DateTimeOffset? dateTimeOffset)
        {
            AggregateIdentifier = aggregate;
            InvocePaidDate = dateTimeOffset;
        }

        public DateTimeOffset? InvocePaidDate { get; set; }
    }
}
