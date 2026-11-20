using System;

using Shift.Common;
using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class EligibilityChanged : Change
    {
        public Guid? Form { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        public ProcessState Process { get; set; }

        public EligibilityChanged(Guid? form, string status, string reason, ProcessState process)
        {
            Form = form;
            Status = status;
            Reason = reason;
            Process = process;
        }
    }
}
