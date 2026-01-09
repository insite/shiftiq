using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionOrderingSolutionChanged : Change
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }
        public decimal Points { get; set; }
        public decimal? CutScore { get; set; }

        public QuestionOrderingSolutionChanged(Guid question, Guid solution, decimal points, decimal? cutScore)
        {
            Question = question;
            Solution = solution;
            Points = points;
            CutScore = cutScore;
        }
    }
}
