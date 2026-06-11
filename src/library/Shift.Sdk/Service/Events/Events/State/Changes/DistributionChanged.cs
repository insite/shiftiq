using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class DistributionChanged : Change
    {
        public string Process { get; set; }

        public DateTimeOffset? Ordered { get; set; }
        public DateTimeOffset? Expected { get; set; }
        public DateTimeOffset? Shipped { get; set; }
        public DateTimeOffset? Used { get; set; }

        public DistributionChanged(string process, DateTimeOffset? ordered, DateTimeOffset? expected, DateTimeOffset? shipped, DateTimeOffset? used)
        {
            Process = process;

            Ordered = ordered;
            Expected = expected;
            Shipped = shipped;
            Used = used;
        }
    }
}