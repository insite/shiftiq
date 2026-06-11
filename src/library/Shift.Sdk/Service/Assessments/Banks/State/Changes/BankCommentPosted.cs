using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class BankCommentPosted : Change
    {
        public Guid Comment { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FlagType Flag { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CommentType Type { get; set; }

        public Guid Subject { get; set; }
        public Guid Author { get; set; }
        public string AuthorRole { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        public DateTimeOffset? Posted { get; set; }
        public Guid? Instructor { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public string EventFormat { get; set; }

        public BankCommentPosted() { }

        public BankCommentPosted(Guid comment, FlagType flag, CommentType type, Guid subject, Guid author, string authorRole, string category, string text, Guid? instructor, DateTimeOffset? eventDate, string format, DateTimeOffset posted)
        {
            Posted = posted;

            Comment = comment;
            Flag = flag;
            Type = type;
            Subject = subject;
            Author = author;
            AuthorRole = authorRole;
            Category = category;
            Text = text;
            Instructor = instructor;
            EventDate = eventDate;
            EventFormat = format;
        }
    }
}
