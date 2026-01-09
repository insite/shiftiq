using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseGroupAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseGroup modify, CaseGroupEntity entity)
    {
        entity.CaseIdentifier = modify.CaseIdentifier;
        entity.GroupIdentifier = modify.GroupIdentifier;
        entity.CaseRole = modify.CaseRole;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

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
            CaseIdentifier = create.CaseIdentifier,
            GroupIdentifier = create.GroupIdentifier,
            CaseRole = create.CaseRole,
            OrganizationIdentifier = create.OrganizationIdentifier,
            JoinIdentifier = create.JoinIdentifier
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
            CaseIdentifier = entity.CaseIdentifier,
            GroupIdentifier = entity.GroupIdentifier,
            CaseRole = entity.CaseRole,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            JoinIdentifier = entity.JoinIdentifier
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
            JoinIdentifier = entity.JoinIdentifier

        };

        return match;
    }
}