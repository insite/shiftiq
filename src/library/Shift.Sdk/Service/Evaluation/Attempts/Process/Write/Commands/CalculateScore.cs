using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Attempts.Write
{
    public class CalculateScore : Command
    {
        public decimal Points { get; set; }
        public decimal Score { get; set; }
        public string Grade { get; set; }
        public bool IsPassing { get; set; }

        public CalculateScore(Guid aggregate, decimal points, decimal score, string grade, bool isPassing)
        {
            AggregateIdentifier = aggregate;
            Points = points;
            Score = score;
            Grade = grade;
            IsPassing = isPassing;
        }
    }
}
