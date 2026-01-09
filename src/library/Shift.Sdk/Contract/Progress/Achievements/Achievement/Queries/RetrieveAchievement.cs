using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAchievement : Query<AchievementModel>
    {
        public Guid AchievementIdentifier { get; set; }
    }
}