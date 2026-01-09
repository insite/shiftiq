
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class ApprovalCompleted : Change
    {
        public string Status { get; set; }
        public string Errors { get; set; }

        public ApprovalCompleted(string status, string errors)
        {
            Status = status;
            Errors = errors;
        }
    }
}
