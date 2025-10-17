using System;

namespace InSite.Application.Attempts.Read
{
    public class QuestionCommentaryItem
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid CommentIdentifier { get; set; }
        public Guid? AssessorUserIdentifier { get; set; }
        public Guid AuthorUserIdentifier { get; set; }
        public DateTimeOffset CommentPosted { get; set; }
        public bool IsSameAssessor { get; set; }
        public bool IsFormThirdPartyAssessmentEnabled { get; set; }
    }
}
