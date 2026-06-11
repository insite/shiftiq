using System;

using InSite.Domain.Records;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Achievements.Write
{
    public class ChangeAchievementNotification : Command
    {
        public NotificationSettings Settings { get; }

        public ChangeAchievementNotification(Guid achievement, NotificationSettings settings)
        {
            AggregateIdentifier = achievement;
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
    }
}
