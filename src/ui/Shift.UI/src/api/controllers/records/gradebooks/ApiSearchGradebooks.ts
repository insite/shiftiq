export interface ApiSearchGradebooks {
    GradebookTitle?: string | null;
    GradebookCreatedSince?: string | null;
    GradebookCreatedBefore?: string | null;
    PeriodId?: string | null;
    AchievementId?: string | null;
    FrameworkId?: string | null;
    IsLocked?: boolean | null;
    ClassTitle?: string | null;
    ClassStartedSince?: string | null;
    ClassStartedBefore?: string | null;
    ClassInstructorId?: string | null;
}