using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionOrderingSolution : Command
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }
        public decimal Points { get; set; }
        public decimal? CutScore { get; set; }

        public ChangeQuestionOrderingSolution(Guid bank, Guid question, Guid solution, decimal points, decimal? cutScore)
        {
            AggregateIdentifier = bank;
            Question = question;
            Solution = solution;
            Points = points;
            CutScore = cutScore;
        }
    }
}
