using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class QuestionScoringChanged : Change
    {
        public Guid Question { get; set; }
        public decimal? Points { get; set; }
        public decimal? CutScore { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public QuestionCalculationMethod CalculationMethod { get; set; }

        public QuestionScoringChanged(Guid question, decimal? points, decimal? cutScore, QuestionCalculationMethod calculationMethod)
        {
            Question = question;
            Points = points;
            CutScore = cutScore;
            CalculationMethod = calculationMethod;
        }
    }
}
