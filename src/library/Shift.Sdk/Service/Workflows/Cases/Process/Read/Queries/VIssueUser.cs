using System;

namespace InSite.Application.Issues.Read
{
    public class VIssueUser
    {
        public Guid IssueIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public String IssueRole { get; set; }
        public String UserFullName { get; set; }

        public virtual VIssue VIssue { get; set; }
    }
}
