using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Organizations
{
    public class OrganizationIssueSettingsModified : Change
    {
        public IssueSettings Issues { get; set; }

        public OrganizationIssueSettingsModified(IssueSettings issues)
        {
            Issues = issues;
        }
    }
}
