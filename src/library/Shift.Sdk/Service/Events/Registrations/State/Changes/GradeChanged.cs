using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class GradeChanged : Change
    {
        public string Grade { get; set; }
        public decimal? Score { get; set; }

        public GradeChanged(string grade, decimal? score)
        {
            Grade = grade;
            Score = score;
        }
    }
}