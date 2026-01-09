using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Registrations
{
    public class SynchronizationChanged : Change
    {
        public string Status { get; set; }

        public ProcessState Process { get; set; }

        public SynchronizationChanged(string status, ProcessState process)
        {
            Status = status;
            Process = process;
        }
    }
}