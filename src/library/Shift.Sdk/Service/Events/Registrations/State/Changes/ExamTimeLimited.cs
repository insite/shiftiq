using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ExamTimeLimited : Change
    {
        public int? Minutes { get; set; }

        public ExamTimeLimited(int? minutes)
        {
            Minutes = minutes;
        }
    }
}
