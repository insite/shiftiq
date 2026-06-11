using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class RecodeSurveyQuestion : Command
    {
        public RecodeSurveyQuestion(Guid form, Guid question, string code, string indicator)
        {
            AggregateIdentifier = form;
            Question = question;
            Code = code;
            Indicator = indicator;
        }

        public Guid Question { get; set; }
        public string Code { get; set; }
        public string Indicator { get; set; }
    }
}