using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Progress;

public class AchievementAdapter : IEntityAdapter
{
    public void Copy(ModifyAchievement modify, AchievementEntity entity)
    {
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.AchievementLabel = modify.AchievementLabel;
        entity.AchievementTitle = modify.AchievementTitle;
        entity.AchievementDescription = modify.AchievementDescription;
        entity.AchievementIsEnabled = modify.AchievementIsEnabled;
        entity.ExpirationType = modify.ExpirationType;
        entity.ExpirationFixedDate = modify.ExpirationFixedDate;
        entity.ExpirationLifetimeQuantity = modify.ExpirationLifetimeQuantity;
        entity.ExpirationLifetimeUnit = modify.ExpirationLifetimeUnit;
        entity.CertificateLayoutCode = modify.CertificateLayoutCode;
        entity.AchievementType = modify.AchievementType;
        entity.AchievementReportingDisabled = modify.AchievementReportingDisabled;
        entity.BadgeImageUrl = modify.BadgeImageUrl;
        entity.HasBadgeImage = modify.HasBadgeImage;
        entity.AchievementAllowSelfDeclared = modify.AchievementAllowSelfDeclared;

    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public AchievementEntity ToEntity(CreateAchievement create)
    {
        var entity = new AchievementEntity
        {
            OrganizationIdentifier = create.OrganizationIdentifier,
            AchievementIdentifier = create.AchievementIdentifier,
            AchievementLabel = create.AchievementLabel,
            AchievementTitle = create.AchievementTitle,
            AchievementDescription = create.AchievementDescription,
            AchievementIsEnabled = create.AchievementIsEnabled,
            ExpirationType = create.ExpirationType,
            ExpirationFixedDate = create.ExpirationFixedDate,
            ExpirationLifetimeQuantity = create.ExpirationLifetimeQuantity,
            ExpirationLifetimeUnit = create.ExpirationLifetimeUnit,
            CertificateLayoutCode = create.CertificateLayoutCode,
            AchievementType = create.AchievementType,
            AchievementReportingDisabled = create.AchievementReportingDisabled,
            BadgeImageUrl = create.BadgeImageUrl,
            HasBadgeImage = create.HasBadgeImage,
            AchievementAllowSelfDeclared = create.AchievementAllowSelfDeclared
        };
        return entity;
    }

    public IEnumerable<AchievementModel> ToModel(IEnumerable<AchievementEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AchievementModel ToModel(AchievementEntity entity)
    {
        var model = new AchievementModel
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            AchievementIdentifier = entity.AchievementIdentifier,
            AchievementLabel = entity.AchievementLabel,
            AchievementTitle = entity.AchievementTitle,
            AchievementDescription = entity.AchievementDescription,
            AchievementIsEnabled = entity.AchievementIsEnabled,
            ExpirationType = entity.ExpirationType,
            ExpirationFixedDate = entity.ExpirationFixedDate,
            ExpirationLifetimeQuantity = entity.ExpirationLifetimeQuantity,
            ExpirationLifetimeUnit = entity.ExpirationLifetimeUnit,
            CertificateLayoutCode = entity.CertificateLayoutCode,
            AchievementType = entity.AchievementType,
            AchievementReportingDisabled = entity.AchievementReportingDisabled,
            BadgeImageUrl = entity.BadgeImageUrl,
            HasBadgeImage = entity.HasBadgeImage,
            AchievementAllowSelfDeclared = entity.AchievementAllowSelfDeclared
        };

        return model;
    }

    public IEnumerable<AchievementMatch> ToMatch(IEnumerable<AchievementEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public AchievementMatch ToMatch(AchievementEntity entity)
    {
        var match = new AchievementMatch
        {
            AchievementId = entity.AchievementIdentifier,
            AchievementTitle = entity.AchievementTitle
        };

        return match;
    }
}