using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Workspace;

public class SiteAdapter : IEntityAdapter
{
    public void Copy(ModifySite modify, SiteEntity entity)
    {
        entity.SiteDomain = modify.SiteDomain;
        entity.SiteTitle = modify.SiteTitle;
        entity.OrganizationIdentifier = modify.OrganizationId;
        entity.LastChangeTime = modify.LastChangeTime;
        entity.LastChangeType = modify.LastChangeType;
        entity.LastChangeUser = modify.LastChangeUser;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public SiteEntity ToEntity(CreateSite create)
    {
        var entity = new SiteEntity
        {
            SiteIdentifier = create.SiteId,
            SiteDomain = create.SiteDomain,
            SiteTitle = create.SiteTitle,
            OrganizationIdentifier = create.OrganizationId,
            LastChangeTime = create.LastChangeTime,
            LastChangeType = create.LastChangeType,
            LastChangeUser = create.LastChangeUser
        };
        return entity;
    }

    public IEnumerable<SiteModel> ToModel(IEnumerable<SiteEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public SiteModel ToModel(SiteEntity entity)
    {
        var model = new SiteModel
        {
            SiteId = entity.SiteIdentifier,
            SiteDomain = entity.SiteDomain,
            SiteTitle = entity.SiteTitle,
            OrganizationId = entity.OrganizationIdentifier,
            LastChangeTime = entity.LastChangeTime,
            LastChangeType = entity.LastChangeType,
            LastChangeUser = entity.LastChangeUser
        };

        return model;
    }

    public IEnumerable<SiteMatch> ToMatch(IEnumerable<SiteEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public SiteMatch ToMatch(SiteEntity entity)
    {
        var match = new SiteMatch
        {
            SiteId = entity.SiteIdentifier

        };

        return match;
    }
}