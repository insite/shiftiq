using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class BankCommentModified : Change
    {
        public Guid Comment { get; set; }
        public Guid Author { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public FlagType Flag { get; set; }

        public string Category { get; set; }
        public string Text { get; set; }
        public Guid? Instructor { get; set; }
        public DateTimeOffset? EventDate { get; set; }
        public string EventFormat { get; set; }

        public DateTimeOffset? Revised { get; set; }

        public BankCommentModified(Guid comment, Guid author, FlagType flag, string category, string text, Guid? instructor, DateTimeOffset? exam, string format, DateTimeOffset? revised)
        {
            Comment = comment;
            Author = author;
            Flag = flag;
            Category = category;
            Text = text;
            Instructor = instructor;
            EventDate = exam;
            EventFormat = format;
            Revised = revised;
        }
    }
}
