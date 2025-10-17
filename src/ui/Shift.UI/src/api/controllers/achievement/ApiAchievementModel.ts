export interface ApiAchievementModel {
    AchievementIdentifier: string;
    OrganizationIdentifier: string;
    AchievementIsEnabled: boolean;
    AchievementReportingDisabled: boolean;
    HasBadgeImage: boolean | null | undefined;
    AchievementDescription: string | null | undefined;
    AchievementLabel: string | null | undefined;
    AchievementTitle: string | null | undefined;
    AchievementType: string | null | undefined;
    BadgeImageUrl: string | null | undefined;
    CertificateLayoutCode: string | null | undefined;
    ExpirationLifetimeUnit: string | null | undefined;
    ExpirationType: string | null | undefined;
    ExpirationLifetimeQuantity: number | null | undefined;
    ExpirationFixedDate: string | null | undefined;
}