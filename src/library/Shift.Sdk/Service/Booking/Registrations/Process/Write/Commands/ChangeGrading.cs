using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Registrations.Write
{
    public class ChangeGrading : Command
    {
        public string Status { get; set; }
        public string Reason { get; set; }
        public ProcessState Process { get; set; }

        public ChangeGrading(Guid aggregate, string status, string reason, ProcessState process)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Reason = reason;
            Process = process ?? new ProcessState();
        }
    }
}
