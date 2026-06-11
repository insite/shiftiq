using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAchievements : Query<int>, IAchievementCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string AchievementTitle { get; set; }

        public DateTimeOffset? ExpirationFixedDateSince { get; set; }
        public DateTimeOffset? ExpirationFixedDateBefore { get; set; }
    }
}
