using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class MerchantAdded : Change
    {
        public MerchantAdded(Guid tenant, string merchant, string description)
        {
            Tenant = tenant;
            Merchant = merchant;
            Description = description;
        }

        public Guid Tenant { get; }
        public string Merchant { get; }
        public string Description { get; }
    }
}