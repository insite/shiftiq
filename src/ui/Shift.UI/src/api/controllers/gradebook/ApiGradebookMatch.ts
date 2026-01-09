export interface ApiGradebookMatch {
    GradebookIdentifier: string;
    GradebookTitle: string;
    GradebookCreated: string;
    GradebookEnrollmentCount: number;
    ClassIdentifier: string | undefined | null;
    ClassTitle: string | undefined | null;
    ClassStarted: string | undefined | null;
    ClassEnded: string | undefined | null;
    AchievementIdentifier: string | undefined | null;
    AchievementTitle: string | undefined | null;
    AchievementCountGranted: number;
    IsLocked: boolean;
}