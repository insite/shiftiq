using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class PeriodRescheduled : Change
    {
        public PeriodRescheduled(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;
        }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}