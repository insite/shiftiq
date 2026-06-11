using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class RecipientAdapter : IEntityAdapter
{
    public void Copy(ModifyRecipient modify, RecipientEntity entity)
    {
        entity.MailoutIdentifier = modify.MailoutId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.UserIdentifier = modify.UserId;
        entity.UserEmail = modify.UserEmail;
        entity.PersonCode = modify.PersonCode;
        entity.PersonName = modify.PersonName;
        entity.PersonLanguage = modify.PersonLanguage;
        entity.RecipientVariables = modify.RecipientVariables;
        entity.DeliveryStarted = modify.DeliveryStarted;
        entity.DeliveryCompleted = modify.DeliveryCompleted;
        entity.DeliveryStatus = modify.DeliveryStatus;
        entity.DeliveryError = modify.DeliveryError;
        entity.DeliveryCallbackTimestamp = modify.DeliveryCallbackTimestamp;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public RecipientEntity ToEntity(CreateRecipient create)
    {
        var entity = new RecipientEntity
        {
            RecipientIdentifier = create.RecipientId,
            MailoutIdentifier = create.MailoutId,
            OrganizationIdentifier = create.OrganizationId,
            UserIdentifier = create.UserId,
            UserEmail = create.UserEmail,
            PersonCode = create.PersonCode,
            PersonName = create.PersonName,
            PersonLanguage = create.PersonLanguage,
            RecipientVariables = create.RecipientVariables,
            DeliveryStarted = create.DeliveryStarted,
            DeliveryCompleted = create.DeliveryCompleted,
            DeliveryStatus = create.DeliveryStatus,
            DeliveryError = create.DeliveryError,
            DeliveryCallbackTimestamp = create.DeliveryCallbackTimestamp
        };

        return entity;
    }

    public IEnumerable<RecipientModel> ToModel(IEnumerable<RecipientEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public RecipientModel ToModel(RecipientEntity entity)
    {
        var model = new RecipientModel
        {
            RecipientId = entity.RecipientIdentifier,
            MailoutId = entity.MailoutIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            UserId = entity.UserIdentifier,
            UserEmail = entity.UserEmail,
            PersonCode = entity.PersonCode,
            PersonName = entity.PersonName,
            PersonLanguage = entity.PersonLanguage,
            RecipientVariables = entity.RecipientVariables,
            DeliveryStarted = entity.DeliveryStarted,
            DeliveryCompleted = entity.DeliveryCompleted,
            DeliveryStatus = entity.DeliveryStatus,
            DeliveryError = entity.DeliveryError,
            DeliveryCallbackTimestamp = entity.DeliveryCallbackTimestamp
        };

        return model;
    }

    public IEnumerable<RecipientMatch> ToMatch(IEnumerable<RecipientEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public RecipientMatch ToMatch(RecipientEntity entity)
    {
        var match = new RecipientMatch
        {
            RecipientId = entity.RecipientIdentifier,
            MailoutId = entity.MailoutIdentifier,
            UserEmail = entity.UserEmail,
            PersonName = entity.PersonName,
            DeliveryStatus = entity.DeliveryStatus
        };

        return match;
    }
}