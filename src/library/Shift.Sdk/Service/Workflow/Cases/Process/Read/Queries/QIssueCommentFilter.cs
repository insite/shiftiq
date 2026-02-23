using System;

using Shift.Common;

namespace InSite.Application.Issues.Read
{
    [Serializable]
    public class QIssueCommentFilter : Filter
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid? IssueIdentifier { get; set; }
        public Guid[] IssueIdentifiers { get; set; }
        public Guid? AuthorIdentifier { get; set; }
        public DateTimeOffset? DatePostedSince { get; set; }
        public DateTimeOffset? DatePostedBefore { get; set; }
        public string ContainerType { get; set; } 
    }
}
