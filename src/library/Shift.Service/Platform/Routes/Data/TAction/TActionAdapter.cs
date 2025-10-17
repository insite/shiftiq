namespace Shift.Service.Metadata;

using Shift.Contract;

using Shift.Common;

public class TActionAdapter : IEntityAdapter
{
    public void Copy(ModifyAction modify, TActionEntity entity)
    {
        entity.NavigationParentActionIdentifier = modify.NavigationParentActionIdentifier;
        entity.PermissionParentActionIdentifier = modify.PermissionParentActionIdentifier;
        entity.ActionIcon = modify.ActionIcon;
        entity.ActionList = modify.ActionList;
        entity.ActionName = modify.ActionName;
        entity.ActionNameShort = modify.ActionNameShort;
        entity.ActionType = modify.ActionType;
        entity.ActionUrl = modify.ActionUrl;
        entity.ControllerPath = modify.ControllerPath;
        entity.HelpUrl = modify.HelpUrl;
        entity.ActionCategory = modify.ActionCategory;
        entity.AuthorizationRequirement = modify.AuthorizationRequirement;

    }

    public TActionEntity ToEntity(CreateAction create)
    {
        var entity = new TActionEntity
        {
            ActionIdentifier = create.ActionIdentifier,
            NavigationParentActionIdentifier = create.NavigationParentActionIdentifier,
            PermissionParentActionIdentifier = create.PermissionParentActionIdentifier,
            ActionIcon = create.ActionIcon,
            ActionList = create.ActionList,
            ActionName = create.ActionName,
            ActionNameShort = create.ActionNameShort,
            ActionType = create.ActionType,
            ActionUrl = create.ActionUrl,
            ControllerPath = create.ControllerPath,
            HelpUrl = create.HelpUrl,
            ActionCategory = create.ActionCategory,
            AuthorizationRequirement = create.AuthorizationRequirement
        };
        return entity;
    }

    public IEnumerable<ActionModel> ToModel(IEnumerable<TActionEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public ActionModel ToModel(TActionEntity entity)
    {
        var model = new ActionModel
        {
            ActionIdentifier = entity.ActionIdentifier,
            NavigationParentActionIdentifier = entity.NavigationParentActionIdentifier,
            PermissionParentActionIdentifier = entity.PermissionParentActionIdentifier,
            ActionIcon = entity.ActionIcon,
            ActionList = entity.ActionList,
            ActionName = entity.ActionName,
            ActionNameShort = entity.ActionNameShort,
            ActionType = entity.ActionType,
            ActionUrl = entity.ActionUrl,
            ControllerPath = entity.ControllerPath,
            HelpUrl = entity.HelpUrl,
            ActionCategory = entity.ActionCategory,
            AuthorizationRequirement = entity.AuthorizationRequirement
        };

        return model;
    }

    public IEnumerable<ActionMatch> ToMatch(IEnumerable<TActionEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public ActionMatch ToMatch(TActionEntity entity)
    {
        var match = new ActionMatch
        {
            ActionIdentifier = entity.ActionIdentifier

        };

        return match;
    }
}