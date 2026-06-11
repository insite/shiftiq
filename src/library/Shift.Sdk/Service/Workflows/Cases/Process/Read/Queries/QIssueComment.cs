using System;

namespace InSite.Application.Issues.Read
{
    public class QIssueComment
    {
        public DateTimeOffset Authored { get; set; }
        public Guid AuthorIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        public String CommentText { get; set; }
        public Guid IssueIdentifier { get; set; }
        public DateTimeOffset? Revised { get; set; }
        public Guid RevisorIdentifier { get; set; }
    }
}
