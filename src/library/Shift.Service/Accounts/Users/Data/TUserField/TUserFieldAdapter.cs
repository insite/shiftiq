namespace Shift.Service.Security;

using Shift.Common;
using Shift.Contract;

public class TUserFieldAdapter : IEntityAdapter
{
    public void Copy(ModifyUserField modify, TUserFieldEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.UserIdentifier = modify.UserId;
        entity.Name = modify.Name;
        entity.ValueType = modify.ValueType;
        entity.ValueJson = modify.ValueJson;

    }

    public TUserFieldEntity ToEntity(CreateUserField create)
    {
        var entity = new TUserFieldEntity
        {
            OrganizationIdentifier = create.OrganizationId,
            UserIdentifier = create.UserId,
            Name = create.Name,
            ValueType = create.ValueType,
            ValueJson = create.ValueJson,
            SettingIdentifier = create.SettingId
        };
        return entity;
    }

    public IEnumerable<UserFieldModel> ToModel(IEnumerable<TUserFieldEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public UserFieldModel ToModel(TUserFieldEntity entity)
    {
        var model = new UserFieldModel
        {
            OrganizationId = entity.OrganizationIdentifier,
            UserId = entity.UserIdentifier,
            Name = entity.Name,
            ValueType = entity.ValueType,
            ValueJson = entity.ValueJson,
            SettingId = entity.SettingIdentifier
        };

        return model;
    }

    public IEnumerable<UserFieldMatch> ToMatch(IEnumerable<TUserFieldEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public UserFieldMatch ToMatch(TUserFieldEntity entity)
    {
        var match = new UserFieldMatch
        {
            SettingId = entity.SettingIdentifier

        };

        return match;
    }
}