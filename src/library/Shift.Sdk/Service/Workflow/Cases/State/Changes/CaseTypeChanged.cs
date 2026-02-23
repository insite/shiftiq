using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseTypeChanged : Change
    {
        public string IssueType { get; set; }

        public CaseTypeChanged(string issueType)
        {
            IssueType = issueType;
        }
    }
}
