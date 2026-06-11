using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeIssueStatus : Command
    {
        public Guid Status { get; set; }
        public DateTimeOffset Effective { get; set; }

        public ChangeIssueStatus(Guid aggregate, Guid status, DateTimeOffset effective)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Effective = effective;
        }
    }
}
