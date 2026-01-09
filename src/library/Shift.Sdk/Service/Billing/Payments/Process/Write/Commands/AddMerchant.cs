using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gateways.Write
{
    public class AddMerchant : Command
    {
        public AddMerchant(Guid gateway, Guid tenant, string merchant, string description)
        {
            AggregateIdentifier = gateway;
            Tenant = tenant;
            Merchant = merchant;
            Description = description;
        }

        public Guid Tenant { get; }
        public string Merchant { get; }
        public string Description { get; }
    }
}