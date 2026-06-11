using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionContentChanged : Change
    {
        public SurveyQuestionContentChanged(Guid question, ContentContainer content)
        {
            Question = question;
            Content = content;
        }

        public Guid Question { get; }
        public ContentContainer Content { get; }
    }
}