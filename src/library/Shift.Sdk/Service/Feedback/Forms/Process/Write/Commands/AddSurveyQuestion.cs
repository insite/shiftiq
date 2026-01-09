using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyQuestion : Command
    {
        public AddSurveyQuestion(Guid form, Guid question, SurveyQuestionType type, string code, string indicator, string source)
        {
            AggregateIdentifier = form;
            Question = question;
            Type = type;
            Code = code;
            Indicator = indicator;
            Source = source;
        }

        public Guid Question { get; }
        public SurveyQuestionType Type { get; }
        public string Code { get; }
        public string Indicator { get; }
        public string Source { get; }
    }
}