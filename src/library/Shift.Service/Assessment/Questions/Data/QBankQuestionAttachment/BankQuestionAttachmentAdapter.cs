using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Assessment;

public class BankQuestionAttachmentAdapter : IEntityAdapter
{
    public void Copy(ModifyBankQuestionAttachment modify, BankQuestionAttachmentEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public BankQuestionAttachmentEntity ToEntity(CreateBankQuestionAttachment create)
    {
        var entity = new BankQuestionAttachmentEntity
        {
            QuestionIdentifier = create.QuestionIdentifier,
            UploadIdentifier = create.UploadIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier
        };
        return entity;
    }

    public IEnumerable<BankQuestionAttachmentModel> ToModel(IEnumerable<BankQuestionAttachmentEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public BankQuestionAttachmentModel ToModel(BankQuestionAttachmentEntity entity)
    {
        var model = new BankQuestionAttachmentModel
        {
            QuestionIdentifier = entity.QuestionIdentifier,
            UploadIdentifier = entity.UploadIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier
        };

        return model;
    }

    public IEnumerable<BankQuestionAttachmentMatch> ToMatch(IEnumerable<BankQuestionAttachmentEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public BankQuestionAttachmentMatch ToMatch(BankQuestionAttachmentEntity entity)
    {
        var match = new BankQuestionAttachmentMatch
        {
            QuestionIdentifier = entity.QuestionIdentifier,
            UploadIdentifier = entity.UploadIdentifier

        };

        return match;
    }
}