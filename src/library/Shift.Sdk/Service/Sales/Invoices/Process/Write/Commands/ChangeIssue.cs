using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Sales.Invoices.Write.Commands
{
    public class ChangeIssue : Command
    {
        public ChangeIssue(Guid invoiceIdentifier, Guid issue)
        {
            AggregateIdentifier = invoiceIdentifier;
            Issue = issue;
        }
        public Guid Issue { get; set; }
    }
}
