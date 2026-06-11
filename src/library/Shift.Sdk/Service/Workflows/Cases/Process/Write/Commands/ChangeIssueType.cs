using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeIssueType : Command
    {
        public string IssueType { get; set; }

        public ChangeIssueType(Guid aggregate, string issueType)
        {
            AggregateIdentifier = aggregate;
            IssueType = issueType;
        }
    }
}
