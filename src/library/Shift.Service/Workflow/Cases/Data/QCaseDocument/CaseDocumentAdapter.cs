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
        entity.IssueIdentifier = modify.CaseIdentifier;
        entity.PosterIdentifier = modify.PosterIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.FileIdentifier = modify.FileIdentifier;

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
            IssueIdentifier = create.CaseIdentifier,
            PosterIdentifier = create.PosterIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            FileIdentifier = create.FileIdentifier,
            AttachmentIdentifier = create.AttachmentIdentifier
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
            CaseIdentifier = entity.IssueIdentifier,
            PosterIdentifier = entity.PosterIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            FileIdentifier = entity.FileIdentifier,
            AttachmentIdentifier = entity.AttachmentIdentifier
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
            AttachmentIdentifier = entity.AttachmentIdentifier

        };

        return match;
    }
}