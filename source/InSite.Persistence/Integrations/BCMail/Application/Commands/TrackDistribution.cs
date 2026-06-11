using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Persistence.Integration.BCMail
{
    public class TrackDistribution : Command
    {
        public readonly bool Test;

        public TrackDistribution(Guid aggregate, bool test)
        {
            AggregateIdentifier = aggregate;
            Test = test;
        }
    }
}
