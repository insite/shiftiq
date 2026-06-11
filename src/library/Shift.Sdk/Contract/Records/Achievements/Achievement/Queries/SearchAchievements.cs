using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchAchievements : Query<IEnumerable<AchievementMatch>>, IAchievementCriteria
    {
        public Guid? OrganizationId { get; set; }

        public string AchievementTitle { get; set; }

        public DateTimeOffset? ExpirationFixedDateSince { get; set; }
        public DateTimeOffset? ExpirationFixedDateBefore { get; set; }
    }
}
