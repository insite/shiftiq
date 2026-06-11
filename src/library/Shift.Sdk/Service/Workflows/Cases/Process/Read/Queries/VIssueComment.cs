using System;

namespace InSite.Application.Issues.Read
{
    public class VIssueComment
    {
        public DateTimeOffset Authored { get; set; }
        public Guid AuthorIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        public String CommentText { get; set; }
        public Guid IssueIdentifier { get; set; }
        public DateTimeOffset? Revised { get; set; }
        public Guid RevisorIdentifier { get; set; }

        public String AuthorName { get; set; }
        public String RevisorName { get; set; }

        public virtual VIssue VIssue { get; set; }
    }
}
