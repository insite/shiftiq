using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyCommentModified : Change
    {
        public Guid Comment { get; set; }
        public string Text { get; set; }
        public FlagType? Flag { get; set; }
        public DateTimeOffset? Resolved { get; set; }

        public SurveyCommentModified(Guid comment, string text, FlagType? flag, DateTimeOffset? resolved)
        {
            Comment = comment;
            Text = text;
            Flag = flag;
            Resolved = resolved;
        }
    }
}
