using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardAchievementRemoved : Change
    {
        public Guid[] AchievementIds { get; }

        public StandardAchievementRemoved(Guid[] achievementIds)
        {
            AchievementIds = achievementIds;
        }
    }
}
