using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAchievement : Query<bool>
    {
        public Guid AchievementId { get; set; }
    }
}