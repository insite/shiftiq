using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workflow;

public class CaseUserAdapter : IEntityAdapter
{
    public void Copy(ModifyCaseUser modify, CaseUserEntity entity)
    {
        entity.CaseIdentifier = modify.CaseId;
        entity.UserIdentifier = modify.UserId;
        entity.CaseRole = modify.CaseRole;
        entity.OrganizationIdentifier = modify.OrganizationId;

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
            CaseIdentifier = create.CaseId,
            UserIdentifier = create.UserId,
            CaseRole = create.CaseRole,
            OrganizationIdentifier = create.OrganizationId,
            JoinIdentifier = create.JoinId
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
            CaseId = entity.CaseIdentifier,
            UserId = entity.UserIdentifier,
            CaseRole = entity.CaseRole,
            OrganizationId = entity.OrganizationIdentifier,
            JoinId = entity.JoinIdentifier
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
            JoinId = entity.JoinIdentifier

        };

        return match;
    }
}