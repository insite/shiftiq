using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Security;

public class QUserConnectionAdapter : IEntityAdapter
{
    public void Copy(ModifyUserConnection modify, QUserConnectionEntity entity)
    {
        entity.Connected = modify.Connected;
        entity.IsManager = modify.IsManager;
        entity.IsSupervisor = modify.IsSupervisor;
        entity.IsValidator = modify.IsValidator;
        entity.IsLeader = modify.IsLeader;

    }

    public string Serialize(IEnumerable<UserConnectionModel> models, string format)
    {
        var content = string.Empty;

        if (format.ToLower() == "csv")
        {
            var csv = new CsvExportHelper(models);

            var properties = typeof(UserConnectionModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => p.Name);

            foreach (var property in properties)
                csv.AddMapping(property, property);

            content = csv.GetString();
        }
        else // The default export file format is JSON.
        {
            content = JsonConvert.SerializeObject(models, Formatting.Indented);
        }

        return content;
    }

    public QUserConnectionEntity ToEntity(CreateUserConnection create)
    {
        var entity = new QUserConnectionEntity
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

    public IEnumerable<UserConnectionModel> ToModel(IEnumerable<QUserConnectionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public UserConnectionModel ToModel(QUserConnectionEntity entity)
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

    public IEnumerable<UserConnectionMatch> ToMatch(IEnumerable<QUserConnectionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public UserConnectionMatch ToMatch(QUserConnectionEntity entity)
    {
        var match = new UserConnectionMatch
        {
            FromUserIdentifier = entity.FromUserIdentifier,
            ToUserIdentifier = entity.ToUserIdentifier

        };

        return match;
    }
}