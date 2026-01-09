using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Gateways.Write
{
    public class OpenGateway : Command
    {
        public OpenGateway(Guid gateway, Guid tenant, GatewayType type, string name)
        {
            AggregateIdentifier = gateway;
            Tenant = tenant;
            Type = type;
            Name = name;
        }

        public Guid Tenant { get; set; }
        public GatewayType Type { get; }
        public string Name { get; }
    }
}
