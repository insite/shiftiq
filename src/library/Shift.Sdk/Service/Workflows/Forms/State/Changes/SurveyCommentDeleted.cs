using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyCommentDeleted : Change
    {
        public Guid Comment { get; set; }

        public SurveyCommentDeleted(Guid comment)
        {
            Comment = comment;
        }
    }
}
