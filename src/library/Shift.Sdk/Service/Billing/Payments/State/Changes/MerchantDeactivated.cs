using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Payments
{
    public class MerchantDeactivated : Change
    {
        public Guid Tenant { get; set; }

        public MerchantDeactivated(Guid tenant)
        {
            Tenant = tenant;
        }
    }
}