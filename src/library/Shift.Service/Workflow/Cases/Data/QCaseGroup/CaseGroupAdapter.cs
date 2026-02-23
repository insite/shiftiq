using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseGroupAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseGroup modify, CaseGroupEntity entity)
    {
        entity.CaseIdentifier = modify.CaseId;
        entity.GroupIdentifier = modify.GroupId;
        entity.CaseRole = modify.CaseRole;
        entity.OrganizationIdentifier = modify.OrganizationId;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CaseGroupEntity ToEntity(CreateCaseGroup create)
    {
        var entity = new CaseGroupEntity
        {
            CaseIdentifier = create.CaseId,
            GroupIdentifier = create.GroupId,
            CaseRole = create.CaseRole,
            OrganizationIdentifier = create.OrganizationId,
            JoinIdentifier = create.JoinId
        };
        return entity;
    }

    public IEnumerable<CaseGroupModel> ToModel(IEnumerable<CaseGroupEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CaseGroupModel ToModel(CaseGroupEntity entity)
    {
        var model = new CaseGroupModel
        {
            CaseId = entity.CaseIdentifier,
            GroupId = entity.GroupIdentifier,
            CaseRole = entity.CaseRole,
            OrganizationId = entity.OrganizationIdentifier,
            JoinId = entity.JoinIdentifier
        };

        return model;
    }

    public IEnumerable<CaseGroupMatch> ToMatch(IEnumerable<CaseGroupEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CaseGroupMatch ToMatch(CaseGroupEntity entity)
    {
        var match = new CaseGroupMatch
        {
            JoinId = entity.JoinIdentifier

        };

        return match;
    }
}