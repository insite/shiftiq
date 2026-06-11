using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CommentAdded : Change
    {
        public Guid Comment { get; }
        public Guid Author { get; }
        public Guid Subject { get; }
        public string SubjectType { get; }
        public string Text { get; }
        public DateTimeOffset Posted { get; }
        public bool IsPrivate { get; }

        public CommentAdded(Guid comment, Guid author, Guid subject, string subjectType, string text, DateTimeOffset posted, bool isPrivate)
        {
            Comment = comment;
            Author = author;
            Subject = subject;
            SubjectType = subjectType;
            Text = text;
            Posted = posted;
            IsPrivate = isPrivate;
        }
    }
}
