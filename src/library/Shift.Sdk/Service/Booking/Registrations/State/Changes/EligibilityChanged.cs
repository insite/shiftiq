using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Registrations
{
    public class EligibilityChanged : Change
    {
        public string Status { get; set; }
        public string Reason { get; set; }

        public ProcessState Process { get; set; }

        public EligibilityChanged(string status, string reason, ProcessState process)
        {
            Status = status;
            Reason = reason;
            Process = process;
        }
    }
}