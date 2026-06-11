using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Registrations.Write
{
    public class ChangeApproval : Command
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public ProcessState Process { get; set; }

        public string PreviousStatus { get; set; }

        public ChangeApproval(Guid aggregate, string status, string reason, ProcessState process, string previous)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Reason = reason;
            Process = process ?? new ProcessState();

            PreviousStatus = previous;
        }
    }
}
