using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Periods.Write
{
    public class ReschedulePeriod : Command
    {
        public ReschedulePeriod(Guid period, DateTimeOffset start, DateTimeOffset end)
        {
            AggregateIdentifier = period;
            Start = start;
            End = end;
        }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}