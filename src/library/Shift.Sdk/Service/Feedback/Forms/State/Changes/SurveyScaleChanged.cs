using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyScaleChanged : Change
    {
        public SurveyScaleChanged(Guid question, SurveyScale scale)
        {
            Question = question;
            Scale = scale;
        }

        public Guid Question { get; set; }
        public SurveyScale Scale { get; }
    }
}