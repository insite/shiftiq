export interface ApiGradebookModel {
    AchievementIdentifier: string | null | undefined;
    EventIdentifier: string | null | undefined;
    FrameworkIdentifier: string | null | undefined;
    GradebookIdentifier: string;
    OrganizationIdentifier: string;
    PeriodIdentifier: string | null | undefined;
    IsLocked: boolean;
    GradebookTitle: string;
    GradebookType: string;
    LastChangeType: string;
    LastChangeUser: string;
    Reference: string | null | undefined;
    GradebookCreated: string;
    LastChangeTime: string;
}