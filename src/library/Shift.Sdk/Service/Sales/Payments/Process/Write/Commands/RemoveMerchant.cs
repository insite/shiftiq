using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gateways.Write
{
    public class RemoveMerchant : Command
    {
        public RemoveMerchant(Guid gateway, Guid tenant)
        {
            AggregateIdentifier = gateway;
            Tenant = tenant;
        }

        public Guid Tenant { get; }
    }
}