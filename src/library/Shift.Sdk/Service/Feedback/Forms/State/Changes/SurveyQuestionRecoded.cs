using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionRecoded : Change
    {
        public SurveyQuestionRecoded(Guid question, string code, string indicator)
        {
            Question = question;
            Code = code;
            Indicator = indicator;
        }

        public Guid Question { get; set; }
        public string Code { get; set; }
        public string Indicator { get; set; }
    }
}