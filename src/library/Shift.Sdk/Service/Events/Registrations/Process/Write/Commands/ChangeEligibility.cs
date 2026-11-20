using System;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ChangeEligibility : Command
    {
        public Guid? Form { get; set; }
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

        public ChangeEligibility(Guid aggregate, Guid form, string status, string reason, ProcessState process = null)
            : this(aggregate, status, reason, process)
        {
            Form = form;
        }
    }
}
