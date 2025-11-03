using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class UserConnectionAdapter : IEntityAdapter
{
    public void Copy(ModifyUserConnection modify, UserConnectionEntity entity)
    {
        entity.Connected = modify.Connected;
        entity.IsManager = modify.IsManager;
        entity.IsSupervisor = modify.IsSupervisor;
        entity.IsValidator = modify.IsValidator;
        entity.IsLeader = modify.IsLeader;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public UserConnectionEntity ToEntity(CreateUserConnection create)
    {
        var entity = new UserConnectionEntity
        {
            Connected = create.Connected,
            IsManager = create.IsManager,
            IsSupervisor = create.IsSupervisor,
            IsValidator = create.IsValidator,
            FromUserIdentifier = create.FromUserIdentifier,
            ToUserIdentifier = create.ToUserIdentifier,
            IsLeader = create.IsLeader
        };
        return entity;
    }

    public IEnumerable<UserConnectionModel> ToModel(IEnumerable<UserConnectionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public UserConnectionModel ToModel(UserConnectionEntity entity)
    {
        var model = new UserConnectionModel
        {
            Connected = entity.Connected,
            IsManager = entity.IsManager,
            IsSupervisor = entity.IsSupervisor,
            IsValidator = entity.IsValidator,
            FromUserIdentifier = entity.FromUserIdentifier,
            ToUserIdentifier = entity.ToUserIdentifier,
            IsLeader = entity.IsLeader
        };

        return model;
    }

    public IEnumerable<UserConnectionMatch> ToMatch(IEnumerable<UserConnectionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public UserConnectionMatch ToMatch(UserConnectionEntity entity)
    {
        var match = new UserConnectionMatch
        {
            FromUserIdentifier = entity.FromUserIdentifier,
            ToUserIdentifier = entity.ToUserIdentifier

        };

        return match;
    }
}