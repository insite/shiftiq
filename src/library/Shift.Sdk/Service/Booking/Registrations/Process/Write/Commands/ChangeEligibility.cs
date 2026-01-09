using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Registrations.Write
{
    public class ChangeEligibility : Command
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public ProcessState Process { get; set; }

        public ChangeEligibility(Guid aggregate, string status, string reason, ProcessState process = null)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Reason = reason;
            Process = process ?? new ProcessState();
        }
    }
}
