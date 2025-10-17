namespace Shift.Service.Site;

using Shift.Contract;

using Shift.Common;

public class QPageAdapter : IEntityAdapter
{
    public void Copy(ModifyPage modify, QPageEntity entity)
    {
        entity.PageType = modify.PageType;
        entity.Title = modify.Title;
        entity.Sequence = modify.Sequence;
        entity.AuthorDate = modify.AuthorDate;
        entity.AuthorName = modify.AuthorName;
        entity.ContentControl = modify.ContentControl;
        entity.NavigateUrl = modify.NavigateUrl;
        entity.IsHidden = modify.IsHidden;
        entity.ContentLabels = modify.ContentLabels;
        entity.PageIcon = modify.PageIcon;
        entity.Hook = modify.Hook;
        entity.IsNewTab = modify.IsNewTab;
        entity.PageSlug = modify.PageSlug;
        entity.SiteIdentifier = modify.SiteIdentifier;
        entity.ParentPageIdentifier = modify.ParentPageIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;
        entity.IsAccessDenied = modify.IsAccessDenied;
        entity.ObjectType = modify.ObjectType;
        entity.ObjectIdentifier = modify.ObjectIdentifier;

    }

    public QPageEntity ToEntity(CreatePage create)
    {
        var entity = new QPageEntity
        {
            PageIdentifier = create.PageIdentifier,
            PageType = create.PageType,
            Title = create.Title,
            Sequence = create.Sequence,
            AuthorDate = create.AuthorDate,
            AuthorName = create.AuthorName,
            ContentControl = create.ContentControl,
            NavigateUrl = create.NavigateUrl,
            IsHidden = create.IsHidden,
            ContentLabels = create.ContentLabels,
            PageIcon = create.PageIcon,
            Hook = create.Hook,
            IsNewTab = create.IsNewTab,
            PageSlug = create.PageSlug,
            SiteIdentifier = create.SiteIdentifier,
            ParentPageIdentifier = create.ParentPageIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser,
            IsAccessDenied = create.IsAccessDenied,
            ObjectType = create.ObjectType,
            ObjectIdentifier = create.ObjectIdentifier
        };
        return entity;
    }

    public IEnumerable<PageModel> ToModel(IEnumerable<QPageEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public PageModel ToModel(QPageEntity entity)
    {
        var model = new PageModel
        {
            PageIdentifier = entity.PageIdentifier,
            PageType = entity.PageType,
            Title = entity.Title,
            Sequence = entity.Sequence,
            AuthorDate = entity.AuthorDate,
            AuthorName = entity.AuthorName,
            ContentControl = entity.ContentControl,
            NavigateUrl = entity.NavigateUrl,
            IsHidden = entity.IsHidden,
            ContentLabels = entity.ContentLabels,
            PageIcon = entity.PageIcon,
            Hook = entity.Hook,
            IsNewTab = entity.IsNewTab,
            PageSlug = entity.PageSlug,
            SiteIdentifier = entity.SiteIdentifier,
            ParentPageIdentifier = entity.ParentPageIdentifier,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser,
            IsAccessDenied = entity.IsAccessDenied,
            ObjectType = entity.ObjectType,
            ObjectIdentifier = entity.ObjectIdentifier
        };

        return model;
    }
}