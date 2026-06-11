using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class ChangeInvoiceNumber : Command
    {
        public ChangeInvoiceNumber(Guid aggregate, int number)
        {
            AggregateIdentifier = aggregate;
            Number = number;
        }

        public int Number { get; set; }
    }
}
