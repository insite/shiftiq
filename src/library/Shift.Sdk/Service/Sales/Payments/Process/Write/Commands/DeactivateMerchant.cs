using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gateways.Write
{
    public class DeactivateMerchant : Command
    {
        public Guid Tenant { get; set; }

        public DeactivateMerchant(Guid gateway, Guid tenant)
        {
            AggregateIdentifier = gateway;
            Tenant = tenant;        
        }
    }
}