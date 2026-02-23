using System;

using Newtonsoft.Json;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CredentialNotificationSent : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CredentialNotificationType NotificationType { get; set; }
        public Guid? LearnerMessageIdentifier { get; set; }
        public Guid? AdministratorMessageIdentifier { get; set; }

        public CredentialNotificationSent(
            CredentialNotificationType notificationType,
            Guid? learnerMessageIdentifier,
            Guid? administratorMessageIdentifier)
        {
            NotificationType = notificationType;
            LearnerMessageIdentifier = learnerMessageIdentifier;
            AdministratorMessageIdentifier = administratorMessageIdentifier;
        }
    }
}
