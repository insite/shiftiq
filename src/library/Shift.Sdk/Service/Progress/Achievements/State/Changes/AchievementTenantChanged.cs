using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementTenantChanged : Change
    {
        public Guid Tenant { get; set; }

        public AchievementTenantChanged(Guid tenant)
        {
            Tenant = tenant;
        }
    }
}
