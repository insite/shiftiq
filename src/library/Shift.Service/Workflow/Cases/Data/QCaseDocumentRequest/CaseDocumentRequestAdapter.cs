using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseDocumentRequestAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseDocumentRequest modify, CaseDocumentRequestEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.RequestedTime = modify.RequestedTime;
        entity.RequestedUserIdentifier = modify.RequestedUserIdentifier;
        entity.RequestedFrom = modify.RequestedFrom;
        entity.RequestedFileSubcategory = modify.RequestedFileSubcategory;
        entity.RequestedFileDescription = modify.RequestedFileDescription;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CaseDocumentRequestEntity ToEntity(CreateCaseDocumentRequest create)
    {
        var entity = new CaseDocumentRequestEntity
        {
            CaseIdentifier = create.CaseIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            RequestedFileCategory = create.RequestedFileCategory,
            RequestedTime = create.RequestedTime,
            RequestedUserIdentifier = create.RequestedUserIdentifier,
            RequestedFrom = create.RequestedFrom,
            RequestedFileSubcategory = create.RequestedFileSubcategory,
            RequestedFileDescription = create.RequestedFileDescription
        };
        return entity;
    }

    public IEnumerable<CaseDocumentRequestModel> ToModel(IEnumerable<CaseDocumentRequestEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CaseDocumentRequestModel ToModel(CaseDocumentRequestEntity entity)
    {
        var model = new CaseDocumentRequestModel
        {
            CaseIdentifier = entity.CaseIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            RequestedFileCategory = entity.RequestedFileCategory,
            RequestedTime = entity.RequestedTime,
            RequestedUserIdentifier = entity.RequestedUserIdentifier,
            RequestedFrom = entity.RequestedFrom,
            RequestedFileSubcategory = entity.RequestedFileSubcategory,
            RequestedFileDescription = entity.RequestedFileDescription
        };

        return model;
    }

    public IEnumerable<CaseDocumentRequestMatch> ToMatch(IEnumerable<CaseDocumentRequestEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CaseDocumentRequestMatch ToMatch(CaseDocumentRequestEntity entity)
    {
        var match = new CaseDocumentRequestMatch
        {
            CaseIdentifier = entity.CaseIdentifier,
            RequestedFileCategory = entity.RequestedFileCategory

        };

        return match;
    }
}