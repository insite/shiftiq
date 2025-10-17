using System;

using Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class SubmitInvoice : Command
    {
        public SubmitInvoice(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}