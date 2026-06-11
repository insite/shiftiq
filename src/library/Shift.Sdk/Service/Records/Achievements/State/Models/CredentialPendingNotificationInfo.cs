using System;

namespace InSite.Domain.Records
{
    public class CredentialPendingNotificationInfo
    {
        public Guid CredentialIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public Guid AchievementIdentifier { get; set; }
        public string AchievementTitle { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public string NotificationType { get; set; }
        public DateTimeOffset NotificationExpected { get; set; }
        public DateTimeOffset CredentialExpirationExpected { get; set; }
        public Guid? LearnerMessageIdentifier { get; set; }
        public Guid? AdministratorMessageIdentifier { get; set; }
    }
}
