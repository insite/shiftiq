using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Registrations
{
    public class GradingChanged : Change
    {
        public string Status { get; set; }
        public string Reason { get; set; }

        public ProcessState Process { get; set; }

        public GradingChanged(string status, string reason = null, ProcessState process = null)
        {
            Status = status;
            Reason = reason;
            Process = process;
        }
    }
}