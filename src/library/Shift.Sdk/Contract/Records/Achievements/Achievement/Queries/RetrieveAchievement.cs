using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAchievement : Query<AchievementModel>
    {
        public Guid AchievementId { get; set; }
    }
}