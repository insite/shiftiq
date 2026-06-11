using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class EventCommentModified : Change
    {
        public Guid CommentIdentifier { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorIdentifier { get; set; }

        public EventCommentModified(Guid comment, Guid author, string text)
        {
            CommentIdentifier = comment;
            AuthorIdentifier = author;
            CommentText = text;
        }
    }
}
