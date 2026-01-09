using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementBadgeImageChanged : Change
    {
        public string BadgeImageUrl { get; set; }

        public AchievementBadgeImageChanged(string badgeImageUrl)
        {
            BadgeImageUrl = badgeImageUrl;
        }
    }
}