using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Invoices.Write
{
    public class ReferenceInvoice : Command
    {
        public Guid? ReferencedInvoice { get; }

        public ReferenceInvoice(Guid invoice, Guid? referencedInvoice)
        {
            AggregateIdentifier = invoice;
            ReferencedInvoice = referencedInvoice;
        }
    }
}
