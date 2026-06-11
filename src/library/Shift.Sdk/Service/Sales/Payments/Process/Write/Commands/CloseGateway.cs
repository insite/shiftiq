using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gateways.Write
{
    public class CloseGateway : Command
    {
        public CloseGateway(Guid gateway)
        {
            AggregateIdentifier = gateway;
        }
    }
}
