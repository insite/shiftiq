using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Registrations
{
    public class ApprovalChanged : Change
    {
        public string Status { get; set; }
        public string Reason { get; set; }

        public string PreviousStatus { get; set; }

        public ProcessState Process { get; set; }

        public ApprovalChanged(string status, string reason, ProcessState process, string previous)
        {
            Status = status;
            Reason = reason;
            Process = process ?? new ProcessState();

            PreviousStatus = previous;
        }
    }
}