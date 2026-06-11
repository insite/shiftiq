using System;

using InSite.Domain.Records;

using Newtonsoft.Json;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class SendCredentialNotification : Command
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public CredentialNotificationType NotificationType { get; set; }
        public Guid? LearnerMessageIdentifier { get; set; }
        public Guid? AdministratorMessageIdentifier { get; set; }

        public SendCredentialNotification(
            Guid credentialId,
            CredentialNotificationType notificationType,
            Guid? learnerMessageIdentifier,
            Guid? administratorMessageIdentifier)
        {
            AggregateIdentifier = credentialId;
            NotificationType = notificationType;
            LearnerMessageIdentifier = learnerMessageIdentifier;
            AdministratorMessageIdentifier = administratorMessageIdentifier;
        }
    }
}
