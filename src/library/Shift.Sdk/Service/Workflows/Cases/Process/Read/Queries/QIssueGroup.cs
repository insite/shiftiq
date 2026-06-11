using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueGroup
    {
        public Guid JoinIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public String IssueRole { get; set; }
    }
}
