export interface ApiGradebookModel {
    AchievementId: string | null | undefined;
    EventId: string | null | undefined;
    FrameworkId: string | null | undefined;
    GradebookId: string;
    OrganizationId: string;
    PeriodId: string | null | undefined;
    IsLocked: boolean;
    GradebookTitle: string;
    GradebookType: string;
    LastChangeType: string;
    LastChangeUser: string;
    Reference: string | null | undefined;
    GradebookCreated: string;
    LastChangeTime: string;
}