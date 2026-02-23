export interface ApiGradebookMatch {
    GradebookId: string;
    GradebookTitle: string;
    GradebookCreated: string;
    GradebookEnrollmentCount: number;
    ClassId: string | undefined | null;
    ClassTitle: string | undefined | null;
    ClassStarted: string | undefined | null;
    ClassEnded: string | undefined | null;
    AchievementId: string | undefined | null;
    AchievementTitle: string | undefined | null;
    AchievementCountGranted: number;
    IsLocked: boolean;
}