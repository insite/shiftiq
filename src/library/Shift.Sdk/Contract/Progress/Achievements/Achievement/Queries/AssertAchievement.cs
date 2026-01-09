using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAchievement : Query<bool>
    {
        public Guid AchievementIdentifier { get; set; }
    }
}