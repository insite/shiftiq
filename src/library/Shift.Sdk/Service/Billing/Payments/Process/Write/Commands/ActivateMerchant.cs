using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Gateways.Write
{
    public class ActivateMerchant : Command
    {
        public ActivateMerchant(Guid gateway, Guid tenant, EnvironmentName environment, string token)
        {
            AggregateIdentifier = gateway;
            Tenant = tenant;
            Environment = environment;
            Token = token;
        }

        public Guid Tenant { get; }
        public EnvironmentName Environment { get; }
        public string Token { get; }
    }
}