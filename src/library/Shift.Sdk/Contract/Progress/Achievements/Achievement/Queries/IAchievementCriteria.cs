using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAchievementCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationIdentifier { get; set; }

        bool? AchievementIsEnabled { get; set; }

        bool? AchievementReportingDisabled { get; set; }

        bool? HasBadgeImage { get; set; }

        string AchievementDescription { get; set; }

        string AchievementLabel { get; set; }

        string AchievementTitle { get; set; }

        string AchievementType { get; set; }

        string BadgeImageUrl { get; set; }

        string CertificateLayoutCode { get; set; }

        string ExpirationLifetimeUnit { get; set; }

        string ExpirationType { get; set; }

        int? ExpirationLifetimeQuantity { get; set; }

        DateTimeOffset? ExpirationFixedDate { get; set; }
    }
}