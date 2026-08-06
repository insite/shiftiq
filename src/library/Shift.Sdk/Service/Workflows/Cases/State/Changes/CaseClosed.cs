using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseClosed : Change
    {
        public DateTimeOffset? Closed { get; set; }

        public CaseClosed(DateTimeOffset? closed)
        {
            Closed = closed;
        }
    }
}
