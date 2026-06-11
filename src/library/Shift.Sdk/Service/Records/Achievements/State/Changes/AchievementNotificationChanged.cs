using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class AchievementNotificationChanged : Change
    {
        public NotificationSettings Settings { get; set; }

        public AchievementNotificationChanged(NotificationSettings settings)
        {
            Settings = settings.Clone();
        }
    }
}
