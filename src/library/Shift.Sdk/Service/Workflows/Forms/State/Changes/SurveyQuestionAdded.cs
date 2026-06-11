using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyQuestionAdded : Change
    {
        public SurveyQuestionAdded(Guid question, SurveyQuestionType type, string code, string indicator, string source)
        {
            Question = question;
            Type = type;
            Code = code;
            Indicator = indicator;
            Source = source;
        }

        public Guid Question { get; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SurveyQuestionType Type { get; }

        public string Code { get; }
        public string Indicator { get; }
        public string Source { get; }
    }
}