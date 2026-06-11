using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Persistence.Integration.BCMail
{
    public class OrderDistribution : Command
    {
        public readonly DistributionRequest Request;
        public readonly bool Test;

        public OrderDistribution(Guid aggregate, DistributionRequest request, bool test)
        {
            AggregateIdentifier = aggregate;
            Request = request;
            Test = test;
        }
    }
}