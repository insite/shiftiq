using System;

namespace InSite.Application.Attempts.Read
{
    [Serializable]
    public class QAttemptCommentExtended
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid AuthorIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }

        public string AuthorName { get; set; }
        public string CommentText { get; set; }

        public DateTimeOffset CommentPosted { get; set; }
    }
}
