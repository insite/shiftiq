using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ChangeIssueTitle : Command
    {
        public string IssueTitle { get; set; }

        public ChangeIssueTitle(Guid aggregate, string issueTitle)
        {
            AggregateIdentifier = aggregate;
            IssueTitle = issueTitle;
        }
    }
}
