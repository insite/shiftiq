using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ReviseComment : Command
    {
        public Guid Comment { get; set; }
        public FlagType Flag { get; set; }
        public Guid Author { get; set; }
        public string Category { get; set; }
        public string Text { get; set; }
        
        public Guid? Instructor { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public string EventFormat { get; set; }

        public DateTimeOffset Revised { get; set; }

        public ReviseComment(Guid bank, Guid comment, Guid author, FlagType flag, string category, string text, Guid? instructor, DateTimeOffset? date, string format, DateTimeOffset revised)
        {
            AggregateIdentifier = bank;
            Comment = comment;
            Flag = flag;
            Author = author;
            Category = category;
            Text = text;
            Instructor = instructor;
            EventDate = date;
            EventFormat = format;
            Revised = revised;
        }
    }
}
