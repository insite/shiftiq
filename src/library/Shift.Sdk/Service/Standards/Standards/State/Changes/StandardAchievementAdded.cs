using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Standards
{
    public class StandardAchievementAdded : Change
    {
        public Guid[] AchievementIds { get; }

        public StandardAchievementAdded(Guid[] achievementIds)
        {
            AchievementIds = achievementIds;
        }
    }
}
