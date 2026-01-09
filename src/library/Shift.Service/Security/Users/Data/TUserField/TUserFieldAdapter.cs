namespace Shift.Service.Security;

using Shift.Contract;

using Shift.Common;

public class TUserFieldAdapter : IEntityAdapter
{
    public void Copy(ModifyUserField modify, TUserFieldEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.UserIdentifier = modify.UserIdentifier;
        entity.Name = modify.Name;
        entity.ValueType = modify.ValueType;
        entity.ValueJson = modify.ValueJson;

    }

    public TUserFieldEntity ToEntity(CreateUserField create)
    {
        var entity = new TUserFieldEntity
        {
            OrganizationIdentifier = create.OrganizationIdentifier,
            UserIdentifier = create.UserIdentifier,
            Name = create.Name,
            ValueType = create.ValueType,
            ValueJson = create.ValueJson,
            SettingIdentifier = create.SettingIdentifier
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
            OrganizationIdentifier = entity.OrganizationIdentifier,
            UserIdentifier = entity.UserIdentifier,
            Name = entity.Name,
            ValueType = entity.ValueType,
            ValueJson = entity.ValueJson,
            SettingIdentifier = entity.SettingIdentifier
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
            SettingIdentifier = entity.SettingIdentifier

        };

        return match;
    }
}