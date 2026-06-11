using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeGrade : Command
    {
        public string Grade { get; set; }
        public decimal? Score { get; set; }

        public ChangeGrade(Guid aggregate, string grade, decimal? score)
        {
            AggregateIdentifier = aggregate;
            Grade = grade;
            Score = score;
        }
    }
}
