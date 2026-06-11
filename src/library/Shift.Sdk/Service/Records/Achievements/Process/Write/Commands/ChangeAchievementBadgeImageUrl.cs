using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementBadgeImageUrl : Command
    {
        public string BadgeImageUrl { get; }

        public ChangeAchievementBadgeImageUrl(Guid achievement, string badgeImageUrl)
        {
            AggregateIdentifier = achievement;
            BadgeImageUrl = badgeImageUrl;
        }
    }
}