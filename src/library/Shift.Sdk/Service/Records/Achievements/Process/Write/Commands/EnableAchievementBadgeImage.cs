using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class EnableAchievementBadgeImage : Command
    {
        public EnableAchievementBadgeImage(Guid achievement)
        {
            AggregateIdentifier = achievement;
        }
    }
}