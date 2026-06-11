using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class MerchantRemoved : Change
    {
        public MerchantRemoved(Guid tenant)
        {
            Tenant = tenant;
        }

        public Guid Tenant { get; }
    }
}