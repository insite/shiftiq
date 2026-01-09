export interface ApiSearchGradebooks {
    GradebookTitle?: string | null;
    GradebookCreatedSince?: string | null;
    GradebookCreatedBefore?: string | null;
    PeriodIdentifier?: string | null;
    AchievementIdentifier?: string | null;
    FrameworkIdentifier?: string | null;
    IsLocked?: boolean | null;
    ClassTitle?: string | null;
    ClassStartedSince?: string | null;
    ClassStartedBefore?: string | null;
    ClassInstructorIdentifier?: string | null;
}