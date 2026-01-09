using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ReviseComment : Command
    {
        public Guid CommentIdentifier { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorIdentifier { get; set; }

        public ReviseComment(Guid id, Guid comment, Guid author, string text)
        {
            AggregateIdentifier = id;
            CommentIdentifier = comment;
            AuthorIdentifier = author;
            CommentText = text;
        }
    }
}
