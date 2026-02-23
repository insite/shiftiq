using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseStatusChanged : Change
    {
        public Guid Status { get; set; }
        public DateTimeOffset Effective { get; set; }

        public CaseStatusChanged(Guid status, DateTimeOffset effective)
        {
            Status = status;
            Effective = effective;
        }
    }
}
