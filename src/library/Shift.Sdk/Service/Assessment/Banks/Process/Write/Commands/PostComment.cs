using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class PostComment : Command
    {
        public Guid Comment { get; set; }
        public FlagType Flag { get; set; }
        public CommentType Type { get; set; }
        public Guid Subject { get; set; }
        public Guid Author { get; set; }
        public string AuthorRole { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public DateTimeOffset Posted { get; set; }
        public Guid? Instructor { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public string EventFormat { get; set; }

        public PostComment(Guid bank, Guid comment, FlagType flag, CommentType type, Guid subject, Guid author, string authorRole, string category, string text, Guid? instructor, DateTimeOffset? date, string format, DateTimeOffset posted)
        {
            Posted = posted;

            AggregateIdentifier = bank;
            Comment = comment;
            Flag = flag;
            Type = type;
            Subject = subject;
            Author = author;
            AuthorRole = authorRole;
            Text = text;
        
            Category = category;
            Instructor = instructor;
            EventDate = date;
            EventFormat = format;
        }
    }
}
