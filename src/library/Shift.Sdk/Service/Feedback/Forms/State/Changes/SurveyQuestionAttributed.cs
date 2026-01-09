using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionAttributed : Change
    {
        public SurveyQuestionAttributed(Guid question, string attribute)
        {
            Question = question;
            Attribute = attribute;
        }

        public Guid Question { get; set; }
        public string Attribute { get; set; }
    }
}