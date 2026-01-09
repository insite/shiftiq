using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sales.Invoices.Changes
{
    public class InvoiceIssueChanged : Change
    {
        public InvoiceIssueChanged(Guid issue)
        {
            Issue = issue;
        }
        public Guid Issue { get; set; }
    }
}
