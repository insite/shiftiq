using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ChangeDistribution : Command
    {
        public string DistributionProcess { get; set; }

        public DateTimeOffset? DistributionOrdered { get; set; }
        public DateTimeOffset? DistributionExpected { get; set; }
        public DateTimeOffset? DistributionShipped { get; set; }
        public DateTimeOffset? AttemptStarted { get; set; }

        public ChangeDistribution(Guid aggregate, string process, DateTimeOffset? ordered, DateTimeOffset? expected, DateTimeOffset? shipped, DateTimeOffset? started)
        {
            AggregateIdentifier = aggregate;

            DistributionProcess = process;

            DistributionOrdered = ordered;
            DistributionExpected = expected;
            DistributionShipped = shipped;
            AttemptStarted = started;
        }
    }
}
