namespace Shift.Service.Security;

using Shift.Contract;

using Shift.Common;

public class TPartitionFieldAdapter : IEntityAdapter
{
    public void Copy(ModifyPartitionField modify, TPartitionFieldEntity entity)
    {
        entity.SettingName = modify.SettingName;
        entity.SettingValue = modify.SettingValue;

    }

    public TPartitionFieldEntity ToEntity(CreatePartitionField create)
    {
        var entity = new TPartitionFieldEntity
        {
            SettingIdentifier = create.SettingIdentifier,
            SettingName = create.SettingName,
            SettingValue = create.SettingValue
        };
        return entity;
    }

    public IEnumerable<PartitionFieldModel> ToModel(IEnumerable<TPartitionFieldEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PartitionFieldModel ToModel(TPartitionFieldEntity entity)
    {
        var model = new PartitionFieldModel
        {
            SettingIdentifier = entity.SettingIdentifier,
            SettingName = entity.SettingName,
            SettingValue = entity.SettingValue
        };

        return model;
    }

    public IEnumerable<PartitionFieldMatch> ToMatch(IEnumerable<TPartitionFieldEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public PartitionFieldMatch ToMatch(TPartitionFieldEntity entity)
    {
        var match = new PartitionFieldMatch
        {
            SettingIdentifier = entity.SettingIdentifier

        };

        return match;
    }
}