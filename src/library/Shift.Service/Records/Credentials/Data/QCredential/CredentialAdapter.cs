using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class CredentialAdapter : IEntityAdapter
{
    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public IEnumerable<CredentialModel> ToModel(IEnumerable<CredentialEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CredentialModel ToModel(CredentialEntity entity)
    {
        var model = new CredentialModel
        {
            AchievementId = entity.AchievementIdentifier,
            UserId = entity.UserIdentifier,
            CredentialGranted = entity.CredentialGranted,
            CredentialRevoked = entity.CredentialRevoked,
            CredentialExpired = entity.CredentialExpired,
            ExpirationType = entity.ExpirationType,
            ExpirationFixedDate = entity.ExpirationFixedDate,
            ExpirationLifetimeQuantity = entity.ExpirationLifetimeQuantity,
            ExpirationLifetimeUnit = entity.ExpirationLifetimeUnit,
            CredentialAssigned = entity.CredentialAssigned,
            CredentialStatus = entity.CredentialStatus,
            CredentialReminderType = entity.CredentialReminderType,
            AuthorityName = entity.AuthorityName,
            AuthorityLocation = entity.AuthorityLocation,
            AuthorityReference = entity.AuthorityReference,
            CredentialDescription = entity.CredentialDescription,
            CredentialHours = entity.CredentialHours,
            CredentialExpirationExpected = entity.CredentialExpirationExpected,
            CredentialExpirationReminderRequested0 = entity.CredentialExpirationReminderRequested0,
            CredentialExpirationReminderRequested1 = entity.CredentialExpirationReminderRequested1,
            CredentialExpirationReminderRequested2 = entity.CredentialExpirationReminderRequested2,
            CredentialExpirationReminderRequested3 = entity.CredentialExpirationReminderRequested3,
            CredentialExpirationReminderDelivered0 = entity.CredentialExpirationReminderDelivered0,
            CredentialExpirationReminderDelivered1 = entity.CredentialExpirationReminderDelivered1,
            CredentialExpirationReminderDelivered2 = entity.CredentialExpirationReminderDelivered2,
            CredentialExpirationReminderDelivered3 = entity.CredentialExpirationReminderDelivered3,
            CredentialId = entity.CredentialIdentifier,
            CredentialNecessity = entity.CredentialNecessity,
            CredentialPriority = entity.CredentialPriority,
            AuthorityId = entity.AuthorityIdentifier,
            AuthorityType = entity.AuthorityType,
            CredentialRevokedReason = entity.CredentialRevokedReason,
            CredentialGrantedDescription = entity.CredentialGrantedDescription,
            CredentialGrantedScore = entity.CredentialGrantedScore,
            CredentialRevokedScore = entity.CredentialRevokedScore,
            TransactionHash = entity.TransactionHash,
            PublisherAddress = entity.PublisherAddress,
            PublicationStatus = entity.PublicationStatus,
            OrganizationId = entity.OrganizationIdentifier,
            EmployerGroupId = entity.EmployerGroupIdentifier,
            EmployerGroupStatus = entity.EmployerGroupStatus,
            BeforeExpiryNotificationSent = entity.BeforeExpiryNotificationSent,
            AfterExpiryNotificationSent = entity.AfterExpiryNotificationSent
        };

        return model;
    }
}
