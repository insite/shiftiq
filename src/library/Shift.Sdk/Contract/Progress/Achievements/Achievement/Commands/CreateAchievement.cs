using System;

namespace Shift.Contract
{
    public class CreateAchievement
    {
        public Guid AchievementIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public bool AchievementAllowSelfDeclared { get; set; }
        public bool AchievementIsEnabled { get; set; }
        public bool AchievementReportingDisabled { get; set; }
        public bool? HasBadgeImage { get; set; }
        public string AchievementDescription { get; set; }
        public string AchievementLabel { get; set; }
        public string AchievementTitle { get; set; }
        public string AchievementType { get; set; }
        public string BadgeImageUrl { get; set; }
        public string CertificateLayoutCode { get; set; }
        public string ExpirationLifetimeUnit { get; set; }
        public string ExpirationType { get; set; }
        public int? ExpirationLifetimeQuantity { get; set; }
        public DateTimeOffset? ExpirationFixedDate { get; set; }
    }
}