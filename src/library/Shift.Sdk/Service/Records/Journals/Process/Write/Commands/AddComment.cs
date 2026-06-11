using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Journals.Write
{
    public class AddComment : Command
    {
        public Guid Comment { get; }
        public Guid Author { get; }
        public Guid Subject { get; }
        public string SubjectType { get; }
        public string Text { get; }
        public DateTimeOffset Posted { get; }
        public bool IsPrivate { get; }

        public AddComment(Guid journal, Guid comment, Guid author, Guid subject, string subjectType, string text, DateTimeOffset posted, bool isPrivate)
        {
            AggregateIdentifier = journal;
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
