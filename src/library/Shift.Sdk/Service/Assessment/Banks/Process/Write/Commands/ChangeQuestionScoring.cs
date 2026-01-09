using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionScoring : Command
    {
        public Guid Question { get; set; }
        public decimal? CutScore { get; set; }
        public decimal? Points { get; set; }
        public QuestionCalculationMethod CalculationMethod { get; set; }

        public ChangeQuestionScoring(Guid bank, Guid question, decimal? points, decimal? cutScore, QuestionCalculationMethod calculationMethod)
        {
            AggregateIdentifier = bank;
            Question = question;
            CutScore = cutScore;
            Points = points;
            CalculationMethod = calculationMethod;
        }
    }
}
