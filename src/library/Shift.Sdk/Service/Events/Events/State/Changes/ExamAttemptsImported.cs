using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamAttemptsImported : Change
    {
        public bool AllowDuplicates { get; set; }

        public ExamAttemptsImported(bool allowDuplicates)
        {
            AllowDuplicates = allowDuplicates;
        }
    }
}
