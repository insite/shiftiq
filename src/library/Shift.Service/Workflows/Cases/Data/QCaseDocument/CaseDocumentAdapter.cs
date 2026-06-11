using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseDocumentAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseDocument modify, CaseDocumentEntity entity)
    {
        entity.FileName = modify.FileName;
        entity.AttachmentPosted = modify.AttachmentPosted;
        entity.FileType = modify.FileType;
        entity.IssueIdentifier = modify.CaseId;
        entity.PosterIdentifier = modify.PosterId;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.FileIdentifier = modify.FileId;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CaseDocumentEntity ToEntity(CreateCaseDocument create)
    {
        var entity = new CaseDocumentEntity
        {
            FileName = create.FileName,
            AttachmentPosted = create.AttachmentPosted,
            FileType = create.FileType,
            IssueIdentifier = create.CaseId,
            PosterIdentifier = create.PosterId,
            OrganizationIdentifier = create.OrganizationId,
            FileIdentifier = create.FileId,
            AttachmentIdentifier = create.AttachmentId
        };
        return entity;
    }

    public IEnumerable<CaseDocumentModel> ToModel(IEnumerable<CaseDocumentEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CaseDocumentModel ToModel(CaseDocumentEntity entity)
    {
        var model = new CaseDocumentModel
        {
            FileName = entity.FileName,
            AttachmentPosted = entity.AttachmentPosted,
            FileType = entity.FileType,
            CaseId = entity.IssueIdentifier,
            PosterId = entity.PosterIdentifier,
            OrganizationId = entity.OrganizationIdentifier,
            FileId = entity.FileIdentifier,
            AttachmentId = entity.AttachmentIdentifier
        };

        return model;
    }

    public IEnumerable<CaseDocumentMatch> ToMatch(IEnumerable<CaseDocumentEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CaseDocumentMatch ToMatch(CaseDocumentEntity entity)
    {
        var match = new CaseDocumentMatch
        {
            AttachmentId = entity.AttachmentIdentifier

        };

        return match;
    }
}