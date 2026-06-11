using System;

namespace InSite.Domain.Records
{
    public class NotificationSettings
    {
        public Guid? BeforeExpiryLearnerMessageIdentifier { get; set; }
        public Guid? BeforeExpiryAdministratorMessageIdentifier { get; set; }
        public int? BeforeExpiryNotificationTiming { get; set; }

        public Guid? AfterExpiryLearnerMessageIdentifier { get; set; }
        public Guid? AfterExpiryAdministratorMessageIdentifier { get; set; }
        public int? AfterExpiryNotificationTiming { get; set; }

        public bool IsEqual(NotificationSettings other)
        {
            return BeforeExpiryLearnerMessageIdentifier == other.BeforeExpiryLearnerMessageIdentifier
                && BeforeExpiryAdministratorMessageIdentifier == other.BeforeExpiryAdministratorMessageIdentifier
                && BeforeExpiryNotificationTiming == other.BeforeExpiryNotificationTiming
                && AfterExpiryLearnerMessageIdentifier == other.AfterExpiryLearnerMessageIdentifier
                && AfterExpiryAdministratorMessageIdentifier == other.AfterExpiryAdministratorMessageIdentifier
                && AfterExpiryNotificationTiming == other.AfterExpiryNotificationTiming;
        }

        public NotificationSettings Clone()
        {
            var result = new NotificationSettings();

            result.Set(this);

            return result;
        }

        internal void Set(NotificationSettings other)
        {
            BeforeExpiryLearnerMessageIdentifier = other.BeforeExpiryLearnerMessageIdentifier;
            BeforeExpiryAdministratorMessageIdentifier = other.BeforeExpiryAdministratorMessageIdentifier;
            BeforeExpiryNotificationTiming = other.BeforeExpiryNotificationTiming;
            AfterExpiryLearnerMessageIdentifier = other.AfterExpiryLearnerMessageIdentifier;
            AfterExpiryAdministratorMessageIdentifier = other.AfterExpiryAdministratorMessageIdentifier;
            AfterExpiryNotificationTiming = other.AfterExpiryNotificationTiming;
        }
    }
}
