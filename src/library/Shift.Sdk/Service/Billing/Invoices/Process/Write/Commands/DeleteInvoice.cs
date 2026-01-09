using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class DeleteInvoice : Command
    {
        public DeleteInvoice(Guid invoice)
        {
            AggregateIdentifier = invoice;
        }
    }
}
