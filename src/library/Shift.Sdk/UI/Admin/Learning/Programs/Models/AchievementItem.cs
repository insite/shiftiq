using System;

namespace Shift.Sdk.UI
{
    public class AchievementItem
    {
        public Guid AchievementIdentifier { get; set; }
        public int? LifetimeMonths { get; set; }
        public bool IsRequired { get; set; }
        public bool IsPlanned { get; set; }
    }
}