using System;

namespace Shift.Sdk.UI
{
    public class AttemptCommentModel
    {
        public int QuestionSequence { get; set; }
        public string QuestionTitle { get; set; }
        public string CommentText { get; set; }
        public DateTimeOffset CommentPosted { get; set; }
    }
}