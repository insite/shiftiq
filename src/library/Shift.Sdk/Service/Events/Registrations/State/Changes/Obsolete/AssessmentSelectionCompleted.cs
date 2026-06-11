
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class AssessmentSelectionCompleted : Change
    {
        public string Status { get; set; }
        public string Reason { get; set; }

        public AssessmentSelectionCompleted(string status, string reason)
        {
            Status = status;
            Reason = reason;
        }
    }
}
