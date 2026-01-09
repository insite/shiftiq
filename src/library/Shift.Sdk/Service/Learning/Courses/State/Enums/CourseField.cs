namespace InSite.Domain.Courses
{
    public enum CourseField
    {
        // Guid
        FrameworkStandardIdentifier,
        GradebookIdentifier,
        CatalogIdentifier,
        CompletionActivityIdentifier,
        CompletedToAdministratorMessageIdentifier,
        CompletedToLearnerMessageIdentifier,
        StalledToAdministratorMessageIdentifier,
        StalledToLearnerMessageIdentifier,
        CreatedBy,
        ModifiedBy,
        SourceIdentifier,

        // string
        CourseCode,
        CourseDescription,
        CourseHook,
        CourseIcon,
        CourseImage,
        CourseLabel,
        CourseLevel,
        CourseName,
        CoursePlatform,
        CourseProgram,
        CourseSlug,
        CourseFlagColor,
        CourseFlagText,
        CourseStyle,
        CourseUnit,

        // bool
        AllowDiscussion,
        CourseIsHidden,
        IsMultipleUnitsEnabled,
        IsProgressReportEnabled,

        // int
        CourseAsset,
        CourseSequence,
        OutlineWidth,
        SendMessageStalledAfterDays,
        SendMessageStalledMaxCount,

        // DatetimeOffset
        Created,
        Modified,
        Closed
    }
}
