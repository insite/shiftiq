using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Surveys.Write
{
    public class PostSurveyComment : Command
    {
        public Guid Comment { get; set; }
        public string Text { get; set; }
        public FlagType? Flag { get; set; }
        public DateTimeOffset? Resolved { get; set; }

        public PostSurveyComment(Guid form, Guid comment, string text, FlagType? flag, DateTimeOffset? resolved)
        {
            AggregateIdentifier = form;
            Comment = comment;
            Text = text;
            Flag = flag;
            Resolved = resolved;
        }
    }
}
