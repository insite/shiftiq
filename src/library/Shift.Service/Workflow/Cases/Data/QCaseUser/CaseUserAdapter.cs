using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseUserAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseUser modify, CaseUserEntity entity)
    {
        entity.CaseIdentifier = modify.CaseIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.CaseRole = modify.CaseRole;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public CaseUserEntity ToEntity(CreateCaseUser create)
    {
        var entity = new CaseUserEntity
        {
            CaseIdentifier = create.CaseIdentifier,
            UserIdentifier = create.UserIdentifier,
            CaseRole = create.CaseRole,
            OrganizationIdentifier = create.OrganizationIdentifier,
            JoinIdentifier = create.JoinIdentifier
        };
        return entity;
    }

    public IEnumerable<CaseUserModel> ToModel(IEnumerable<CaseUserEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public CaseUserModel ToModel(CaseUserEntity entity)
    {
        var model = new CaseUserModel
        {
            CaseIdentifier = entity.CaseIdentifier,
            UserIdentifier = entity.UserIdentifier,
            CaseRole = entity.CaseRole,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            JoinIdentifier = entity.JoinIdentifier
        };

        return model;
    }

    public IEnumerable<CaseUserMatch> ToMatch(IEnumerable<CaseUserEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public CaseUserMatch ToMatch(CaseUserEntity entity)
    {
        var match = new CaseUserMatch
        {
            JoinIdentifier = entity.JoinIdentifier

        };

        return match;
    }
}