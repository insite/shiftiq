using System;

using InSite.Application.Credentials.Write;
using InSite.Application.Messages.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Messages;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Records.Write
{
    public class CredentialNotificationHelper
    {
        private readonly IAchievementSearch _achievementSearch;
        private readonly Action<ICommand> _sendCommand;
        private readonly IAlertMailer _alertMailer;

        public CredentialNotificationHelper(
            IAchievementSearch achievementSearch,
            Action<ICommand> sendCommand,
            IAlertMailer alertMailer
        )
        {
            _achievementSearch = achievementSearch;
            _sendCommand = sendCommand;
            _alertMailer = alertMailer;
        }

        public int CreateNotifications()
        {
            var now = DateTime.Now;
            var data = _achievementSearch.GetCredentialsPendingNotification(now);
            var result = 0;

            foreach (var item in data)
            {
                if (item.NotificationType == "BeforeExpiry")
                    result += ProceedBeforeExpiry(item);
                else if (item.NotificationType == "AfterExpiry")
                    result += ProceedAfterExpiry(item);
            }

            return result;
        }

        private int ProceedBeforeExpiry(CredentialPendingNotificationInfo info)
        {
            var result = 0;

            if (info.LearnerMessageIdentifier.HasValue)
            {
                var notification = new AchievementExpiringNotification
                {
                    OriginOrganization = info.OrganizationIdentifier,
                    MessageIdentifier = info.LearnerMessageIdentifier.Value,
                    Scheduled = info.NotificationExpected,
                    AchievementTitle = info.AchievementTitle,
                    AchievementExpirationDate = info.CredentialExpirationExpected.Format()
                };

                var mailoutIds = _alertMailer.Send(notification, info.UserIdentifier);

                result += mailoutIds.Length;
            }

            if (info.AdministratorMessageIdentifier.HasValue)
            {
                var notification = new LearnerAchievementExpiringNotification
                {
                    OriginOrganization = info.OrganizationIdentifier,
                    MessageIdentifier = info.AdministratorMessageIdentifier.Value,
                    Scheduled = info.NotificationExpected,
                    AchievementTitle = info.AchievementTitle,
                    AchievementExpirationDate = info.CredentialExpirationExpected.Format(),
                    LearnerIdentifier = info.UserIdentifier,
                    LearnerName = info.UserFullName,
                };

                var mailoutIds = _alertMailer.Send(notification);

                result += mailoutIds.Length;
            }

            _sendCommand(new SendCredentialNotification(
                info.CredentialIdentifier,
                CredentialNotificationType.BeforeExpiry,
                info.LearnerMessageIdentifier, info.AdministratorMessageIdentifier));

            return result;
        }

        private int ProceedAfterExpiry(CredentialPendingNotificationInfo info)
        {
            var result = 0;

            if (info.LearnerMessageIdentifier.HasValue)
            {
                var notification = new AchievementExpiredNotification
                {
                    OriginOrganization = info.OrganizationIdentifier,
                    MessageIdentifier = info.LearnerMessageIdentifier.Value,
                    Scheduled = info.NotificationExpected,
                    AchievementTitle = info.AchievementTitle,
                    AchievementExpirationDate = info.CredentialExpirationExpected.Format()
                };

                var mailoutIds = _alertMailer.Send(notification, info.UserIdentifier);

                result += mailoutIds.Length;
            }

            if (info.AdministratorMessageIdentifier.HasValue)
            {
                var notification = new LearnerAchievementExpiredNotification
                {
                    OriginOrganization = info.OrganizationIdentifier,
                    MessageIdentifier = info.AdministratorMessageIdentifier.Value,
                    Scheduled = info.NotificationExpected,
                    AchievementTitle = info.AchievementTitle,
                    AchievementExpirationDate = info.CredentialExpirationExpected.Format(),
                    LearnerIdentifier = info.UserIdentifier,
                    LearnerName = info.UserFullName,
                };

                var mailoutIds = _alertMailer.Send(notification);

                result += mailoutIds.Length;
            }

            _sendCommand(new SendCredentialNotification(
                info.CredentialIdentifier,
                CredentialNotificationType.AfterExpiry,
                info.LearnerMessageIdentifier, info.AdministratorMessageIdentifier));

            return result;
        }
    }
}
