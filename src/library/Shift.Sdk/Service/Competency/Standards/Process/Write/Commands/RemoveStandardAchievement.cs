using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class RemoveStandardAchievement : Command
    {
        public Guid[] AchievementIds { get; set; }

        public RemoveStandardAchievement(Guid standardId, Guid achievementId)
            : this(standardId, new[] { achievementId })
        {
        }

        public RemoveStandardAchievement(Guid standardId, Guid[] achievementIds)
        {
            AggregateIdentifier = standardId;
            AchievementIds = achievementIds;
        }
    }
}
