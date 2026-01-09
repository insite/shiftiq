using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionDeleted : Change
    {
        public SurveyQuestionDeleted(Guid question)
        {
            Question = question;
        }

        public Guid Question { get; set; }
    }
}