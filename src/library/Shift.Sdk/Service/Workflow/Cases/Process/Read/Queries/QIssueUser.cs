using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueUser
    {
        public Guid JoinIdentifier { get; set; }
        public Guid IssueIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public String IssueRole { get; set; }
    }
}
