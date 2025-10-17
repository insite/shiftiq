using Shift.Contract;

using Shift.Common;

namespace Shift.Service.Achievement;

public class QAchievementAdapter : IEntityAdapter
{
    public void Copy(ModifyAchievement modify, QAchievementEntity entity)
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

    }

    public QAchievementEntity ToEntity(CreateAchievement create)
    {
        var entity = new QAchievementEntity
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
            HasBadgeImage = create.HasBadgeImage
        };
        return entity;
    }

    public IEnumerable<AchievementModel> ToModel(IEnumerable<QAchievementEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public AchievementModel ToModel(QAchievementEntity entity)
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
            HasBadgeImage = entity.HasBadgeImage
        };

        return model;
    }
}