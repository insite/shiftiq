using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class AddQuestionOrderingSolution : Command
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }
        public decimal Points { get; set; }
        public decimal? CutScore { get; set; }

        public AddQuestionOrderingSolution(Guid bank, Guid question, Guid solution, decimal points, decimal? cutScore)
        {
            AggregateIdentifier = bank;
            Question = question;
            Solution = solution;
            Points = points;
            CutScore = cutScore;
        }
    }
}
