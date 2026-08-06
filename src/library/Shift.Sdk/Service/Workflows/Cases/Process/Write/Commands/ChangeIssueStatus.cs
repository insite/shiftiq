using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeIssueStatus : Command
    {
        public Guid Status { get; set; }
        public DateTimeOffset Effective { get; set; }
        public string Category { get; set; }

        public ChangeIssueStatus(Guid aggregate, Guid status, DateTimeOffset effective, string category)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Effective = effective;
            Category = category;
        }
    }
}
