using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Standards.Write
{
    public class AddStandardAchievement : Command
    {
        public Guid[] AchievementIds { get; set; }

        public AddStandardAchievement(Guid standardId, Guid achievementId)
            : this(standardId, new[] { achievementId })
        {
        }

        public AddStandardAchievement(Guid standardId, Guid[] achievementIds)
        {
            AggregateIdentifier = standardId;
            AchievementIds = achievementIds;
        }
    }
}
