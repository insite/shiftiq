using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAchievementCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        string AchievementTitle { get; set; }

        DateTimeOffset? ExpirationFixedDateSince { get; set; }
        DateTimeOffset? ExpirationFixedDateBefore { get; set; }
    }
}
