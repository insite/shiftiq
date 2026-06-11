using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Messaging;

public class MailoutAdapter : IEntityAdapter
{
    public void Copy(ModifyMailout modify, MailoutEntity entity)
    {
        entity.EventIdentifier = modify.EventId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.SurveyIdentifier = modify.SurveyId;
        entity.UserIdentifier = modify.UserId;
        entity.SenderIdentifier = modify.SenderId;
        entity.MessageIdentifier = modify.MessageId;
        entity.SenderStatus = modify.SenderStatus;
        entity.SenderType = modify.SenderType;
        entity.MessageType = modify.MessageType;
        entity.MessageName = modify.MessageName;
        entity.ContentBodyHtml = modify.ContentBodyHtml;
        entity.ContentBodyText = modify.ContentBodyText;
        entity.ContentPriority = modify.ContentPriority;
        entity.ContentSubject = modify.ContentSubject;
        entity.ContentVariables = modify.ContentVariables;
        entity.ContentAttachments = modify.ContentAttachments;
        entity.MailoutStatus = modify.MailoutStatus;
        entity.MailoutStatusCode = modify.MailoutStatusCode;
        entity.MailoutStatusDescription = modify.MailoutStatusDescription;
        entity.MailoutError = modify.MailoutError;
        entity.RecipientEmailsTo = modify.RecipientEmailsTo;
        entity.RecipientEmailsCc = modify.RecipientEmailsCc;
        entity.RecipientEmailsBcc = modify.RecipientEmailsBcc;
        entity.RecipientIdentifiersTo = modify.RecipientIdentifiersTo;
        entity.RecipientIdentifiersCc = modify.RecipientIdentifiersCc;
        entity.RecipientIdentifiersBcc = modify.RecipientIdentifiersBcc;
        entity.RecipientListTo = modify.RecipientListTo;
        entity.RecipientListCc = modify.RecipientListCc;
        entity.RecipientListBcc = modify.RecipientListBcc;
        entity.MailoutScheduled = modify.MailoutScheduled;
        entity.MailoutStarted = modify.MailoutStarted;
        entity.MailoutCancelled = modify.MailoutCancelled;
        entity.MailoutCompleted = modify.MailoutCompleted;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public MailoutEntity ToEntity(CreateMailout create)
    {
        var entity = new MailoutEntity
        {
            MailoutIdentifier = create.MailoutId,
            EventIdentifier = create.EventId,
            OrganizationIdentifier = create.OrganizationId,
            SurveyIdentifier = create.SurveyId,
            UserIdentifier = create.UserId,
            SenderIdentifier = create.SenderId,
            MessageIdentifier = create.MessageId,
            SenderStatus = create.SenderStatus,
            SenderType = create.SenderType,
            MessageType = create.MessageType,
            MessageName = create.MessageName,
            ContentBodyHtml = create.ContentBodyHtml,
            ContentBodyText = create.ContentBodyText,
            ContentPriority = create.ContentPriority,
            ContentSubject = create.ContentSubject,
            ContentVariables = create.ContentVariables,
            ContentAttachments = create.ContentAttachments,
            MailoutStatus = create.MailoutStatus,
            MailoutStatusCode = create.MailoutStatusCode,
            MailoutStatusDescription = create.MailoutStatusDescription,
            MailoutError = create.MailoutError,
            RecipientEmailsTo = create.RecipientEmailsTo,
            RecipientEmailsCc = create.RecipientEmailsCc,
            RecipientEmailsBcc = create.RecipientEmailsBcc,
            RecipientIdentifiersTo = create.RecipientIdentifiersTo,
            RecipientIdentifiersCc = create.RecipientIdentifiersCc,
            RecipientIdentifiersBcc = create.RecipientIdentifiersBcc,
            RecipientListTo = create.RecipientListTo,
            RecipientListCc = create.RecipientListCc,
            RecipientListBcc = create.RecipientListBcc,
            MailoutScheduled = create.MailoutScheduled,
            MailoutStarted = create.MailoutStarted,
            MailoutCancelled = create.MailoutCancelled,
            MailoutCompleted = create.MailoutCompleted
        };

        return entity;
    }

    public IEnumerable<MailoutModel> ToModel(IEnumerable<MailoutEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public MailoutModel ToModel(MailoutEntity entity)
    {
        var model = new MailoutModel
        {
            MailoutId = entity.MailoutIdentifier,
            EventId = entity.EventIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            SurveyId = entity.SurveyIdentifier,
            UserId = entity.UserIdentifier,
            SenderId = entity.SenderIdentifier,
            MessageId = entity.MessageIdentifier,
            SenderStatus = entity.SenderStatus,
            SenderType = entity.SenderType,
            MessageType = entity.MessageType,
            MessageName = entity.MessageName,
            ContentBodyHtml = entity.ContentBodyHtml,
            ContentBodyText = entity.ContentBodyText,
            ContentPriority = entity.ContentPriority,
            ContentSubject = entity.ContentSubject,
            ContentVariables = entity.ContentVariables,
            ContentAttachments = entity.ContentAttachments,
            MailoutStatus = entity.MailoutStatus,
            MailoutStatusCode = entity.MailoutStatusCode,
            MailoutStatusDescription = entity.MailoutStatusDescription,
            MailoutError = entity.MailoutError,
            RecipientEmailsTo = entity.RecipientEmailsTo,
            RecipientEmailsCc = entity.RecipientEmailsCc,
            RecipientEmailsBcc = entity.RecipientEmailsBcc,
            RecipientIdentifiersTo = entity.RecipientIdentifiersTo,
            RecipientIdentifiersCc = entity.RecipientIdentifiersCc,
            RecipientIdentifiersBcc = entity.RecipientIdentifiersBcc,
            RecipientListTo = entity.RecipientListTo,
            RecipientListCc = entity.RecipientListCc,
            RecipientListBcc = entity.RecipientListBcc,
            MailoutScheduled = entity.MailoutScheduled,
            MailoutStarted = entity.MailoutStarted,
            MailoutCancelled = entity.MailoutCancelled,
            MailoutCompleted = entity.MailoutCompleted
        };

        return model;
    }

    public IEnumerable<MailoutMatch> ToMatch(IEnumerable<MailoutEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public MailoutMatch ToMatch(MailoutEntity entity)
    {
        var match = new MailoutMatch
        {
            MailoutId = entity.MailoutIdentifier,
            MessageId = entity.MessageIdentifier,
            MessageName = entity.MessageName,
            MailoutStatus = entity.MailoutStatus,
            ContentSubject = entity.ContentSubject,
            MailoutScheduled = entity.MailoutScheduled
        };

        return match;
    }
}