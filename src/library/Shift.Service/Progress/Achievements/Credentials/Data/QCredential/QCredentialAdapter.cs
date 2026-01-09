using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class QCredentialAdapter : IEntityAdapter
{
    public void Copy(ModifyCredential modify, QCredentialEntity entity)
    {
        entity.AchievementIdentifier = modify.AchievementIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.CredentialGranted = modify.CredentialGranted;
        entity.CredentialRevoked = modify.CredentialRevoked;
        entity.CredentialExpired = modify.CredentialExpired;
        entity.ExpirationType = modify.ExpirationType;
        entity.ExpirationFixedDate = modify.ExpirationFixedDate;
        entity.ExpirationLifetimeQuantity = modify.ExpirationLifetimeQuantity;
        entity.ExpirationLifetimeUnit = modify.ExpirationLifetimeUnit;
        entity.CredentialAssigned = modify.CredentialAssigned;
        entity.CredentialStatus = modify.CredentialStatus;
        entity.CredentialReminderType = modify.CredentialReminderType;
        entity.AuthorityName = modify.AuthorityName;
        entity.AuthorityLocation = modify.AuthorityLocation;
        entity.AuthorityReference = modify.AuthorityReference;
        entity.CredentialDescription = modify.CredentialDescription;
        entity.CredentialHours = modify.CredentialHours;
        entity.CredentialExpirationExpected = modify.CredentialExpirationExpected;
        entity.CredentialExpirationReminderRequested0 = modify.CredentialExpirationReminderRequested0;
        entity.CredentialExpirationReminderRequested1 = modify.CredentialExpirationReminderRequested1;
        entity.CredentialExpirationReminderRequested2 = modify.CredentialExpirationReminderRequested2;
        entity.CredentialExpirationReminderRequested3 = modify.CredentialExpirationReminderRequested3;
        entity.CredentialExpirationReminderDelivered0 = modify.CredentialExpirationReminderDelivered0;
        entity.CredentialExpirationReminderDelivered1 = modify.CredentialExpirationReminderDelivered1;
        entity.CredentialExpirationReminderDelivered2 = modify.CredentialExpirationReminderDelivered2;
        entity.CredentialExpirationReminderDelivered3 = modify.CredentialExpirationReminderDelivered3;
        entity.CredentialNecessity = modify.CredentialNecessity;
        entity.CredentialPriority = modify.CredentialPriority;
        entity.AuthorityIdentifier = modify.AuthorityIdentifier;
        entity.AuthorityType = modify.AuthorityType;
        entity.CredentialRevokedReason = modify.CredentialRevokedReason;
        entity.CredentialGrantedDescription = modify.CredentialGrantedDescription;
        entity.CredentialGrantedScore = modify.CredentialGrantedScore;
        entity.CredentialRevokedScore = modify.CredentialRevokedScore;
        entity.TransactionHash = modify.TransactionHash;
        entity.PublisherAddress = modify.PublisherAddress;
        entity.PublicationStatus = modify.PublicationStatus;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.EmployerGroupIdentifier = modify.EmployerGroupIdentifier;
        entity.EmployerGroupStatus = modify.EmployerGroupStatus;

    }

    public QCredentialEntity ToEntity(CreateCredential create)
    {
        var entity = new QCredentialEntity
        {
            AchievementIdentifier = create.AchievementIdentifier,
            UserIdentifier = create.UserIdentifier,
            CredentialGranted = create.CredentialGranted,
            CredentialRevoked = create.CredentialRevoked,
            CredentialExpired = create.CredentialExpired,
            ExpirationType = create.ExpirationType,
            ExpirationFixedDate = create.ExpirationFixedDate,
            ExpirationLifetimeQuantity = create.ExpirationLifetimeQuantity,
            ExpirationLifetimeUnit = create.ExpirationLifetimeUnit,
            CredentialAssigned = create.CredentialAssigned,
            CredentialStatus = create.CredentialStatus,
            CredentialReminderType = create.CredentialReminderType,
            AuthorityName = create.AuthorityName,
            AuthorityLocation = create.AuthorityLocation,
            AuthorityReference = create.AuthorityReference,
            CredentialDescription = create.CredentialDescription,
            CredentialHours = create.CredentialHours,
            CredentialExpirationExpected = create.CredentialExpirationExpected,
            CredentialExpirationReminderRequested0 = create.CredentialExpirationReminderRequested0,
            CredentialExpirationReminderRequested1 = create.CredentialExpirationReminderRequested1,
            CredentialExpirationReminderRequested2 = create.CredentialExpirationReminderRequested2,
            CredentialExpirationReminderRequested3 = create.CredentialExpirationReminderRequested3,
            CredentialExpirationReminderDelivered0 = create.CredentialExpirationReminderDelivered0,
            CredentialExpirationReminderDelivered1 = create.CredentialExpirationReminderDelivered1,
            CredentialExpirationReminderDelivered2 = create.CredentialExpirationReminderDelivered2,
            CredentialExpirationReminderDelivered3 = create.CredentialExpirationReminderDelivered3,
            CredentialIdentifier = create.CredentialIdentifier,
            CredentialNecessity = create.CredentialNecessity,
            CredentialPriority = create.CredentialPriority,
            AuthorityIdentifier = create.AuthorityIdentifier,
            AuthorityType = create.AuthorityType,
            CredentialRevokedReason = create.CredentialRevokedReason,
            CredentialGrantedDescription = create.CredentialGrantedDescription,
            CredentialGrantedScore = create.CredentialGrantedScore,
            CredentialRevokedScore = create.CredentialRevokedScore,
            TransactionHash = create.TransactionHash,
            PublisherAddress = create.PublisherAddress,
            PublicationStatus = create.PublicationStatus,
            OrganizationIdentifier = create.OrganizationIdentifier,
            EmployerGroupIdentifier = create.EmployerGroupIdentifier,
            EmployerGroupStatus = create.EmployerGroupStatus
        };
        return entity;
    }

    public IEnumerable<CredentialModel> ToModel(IEnumerable<QCredentialEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CredentialModel ToModel(QCredentialEntity entity)
    {
        var model = new CredentialModel
        {
            AchievementIdentifier = entity.AchievementIdentifier,
            UserIdentifier = entity.UserIdentifier,
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
            CredentialIdentifier = entity.CredentialIdentifier,
            CredentialNecessity = entity.CredentialNecessity,
            CredentialPriority = entity.CredentialPriority,
            AuthorityIdentifier = entity.AuthorityIdentifier,
            AuthorityType = entity.AuthorityType,
            CredentialRevokedReason = entity.CredentialRevokedReason,
            CredentialGrantedDescription = entity.CredentialGrantedDescription,
            CredentialGrantedScore = entity.CredentialGrantedScore,
            CredentialRevokedScore = entity.CredentialRevokedScore,
            TransactionHash = entity.TransactionHash,
            PublisherAddress = entity.PublisherAddress,
            PublicationStatus = entity.PublicationStatus,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            EmployerGroupIdentifier = entity.EmployerGroupIdentifier,
            EmployerGroupStatus = entity.EmployerGroupStatus
        };

        return model;
    }

    public IEnumerable<CredentialMatch> ToMatch(IEnumerable<QCredentialEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CredentialMatch ToMatch(QCredentialEntity entity)
    {
        var match = new CredentialMatch
        {
            CredentialIdentifier = entity.CredentialIdentifier

        };

        return match;
    }
}