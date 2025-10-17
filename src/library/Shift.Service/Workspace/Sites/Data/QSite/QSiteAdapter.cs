namespace Shift.Service.Site;

using Shift.Contract;

using Shift.Common;

public class QSiteAdapter : IEntityAdapter
{
    public void Copy(ModifySite modify, QSiteEntity entity)
    {
        entity.SiteDomain = modify.SiteDomain;
        entity.SiteTitle = modify.SiteTitle;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;

    }

    public QSiteEntity ToEntity(CreateSite create)
    {
        var entity = new QSiteEntity
        {
            SiteIdentifier = create.SiteIdentifier,
            SiteDomain = create.SiteDomain,
            SiteTitle = create.SiteTitle,
            OrganizationIdentifier = create.OrganizationIdentifier,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser
        };
        return entity;
    }

    public IEnumerable<SiteModel> ToModel(IEnumerable<QSiteEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public SiteModel ToModel(QSiteEntity entity)
    {
        var model = new SiteModel
        {
            SiteIdentifier = entity.SiteIdentifier,
            SiteDomain = entity.SiteDomain,
            SiteTitle = entity.SiteTitle,
            OrganizationIdentifier = entity.OrganizationIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser
        };

        return model;
    }

    public IEnumerable<SiteMatch> ToMatch(IEnumerable<QSiteEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public SiteMatch ToMatch(QSiteEntity entity)
    {
        var match = new SiteMatch
        {
            SiteIdentifier = entity.SiteIdentifier

        };

        return match;
    }
}