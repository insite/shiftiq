using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseTitleChanged : Change
    {
        public string IssueTitle { get; set; }

        public CaseTitleChanged(string issueTitle)
        {
            IssueTitle = issueTitle;
        }
    }
}
