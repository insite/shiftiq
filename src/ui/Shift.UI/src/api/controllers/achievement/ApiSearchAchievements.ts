export interface ApiSearchAchievements {
    OrganizationId?: string | null;
    AchievementId?: string | null;
    AchievementIsEnabled?: boolean | null;
    AchievementReportingDisabled?: boolean | null;
    HasBadgeImage?: boolean | null;
    AchievementDescription?: string | null;
    AchievementLabel?: string | null;
    AchievementTitle?: string | null;
    AchievementType?: string | null;
    BadgeImageUrl?: string | null;
    CertificateLayoutCode?: string | null;
    ExpirationLifetimeUnit?: string | null;
    ExpirationType?: string | null;
    ExpirationLifetimeQuantity?: number | null;
    ExpirationFixedDate?: number | null;
}