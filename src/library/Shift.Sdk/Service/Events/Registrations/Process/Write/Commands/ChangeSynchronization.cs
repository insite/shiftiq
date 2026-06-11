using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Registrations.Write
{
    public class ChangeSynchronization : Command
    {
        public string Status { get; set; }
        public ProcessState Process { get; set; }

        public ChangeSynchronization(Guid aggregate, string status, ProcessState process)
        {
            AggregateIdentifier = aggregate;
            Status = status;
            Process = process ?? new ProcessState();
        }
    }
}
