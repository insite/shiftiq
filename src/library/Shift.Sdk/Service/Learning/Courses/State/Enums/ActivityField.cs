namespace InSite.Domain.Courses
{
    public enum ActivityField
    {
        // Guid
        AssessmentFormIdentifier,
        GradeItemIdentifier,
        PrerequisiteActivityIdentifier,
        SurveyFormIdentifier,
        QuizIdentifier,
        CreatedBy,
        ModifiedBy,
        SourceIdentifier,

        // string
        ActivityAuthorName,
        ActivityCode,
        ActivityHook,
        ActivityImage,
        ActivityMode,
        ActivityName,
        ActivityPlatform,
        ActivityUrl,
        ActivityUrlTarget,
        ActivityUrlType,
        PrerequisiteDeterminer,
        RequirementCondition,
        DoneButtonText,
        DoneButtonInstructions,
        DoneMarkedInstructions,

        // bool
        ActivityIsMultilingual,
        ActivityIsAdaptive,
        ActivityIsDispatch,

        // int
        ActivityAsset,
        ActivityMinutes,
        ActivitySequence,

        // DateTimeOffset
        Created,
        Modified,

        // DateTime
        ActivityAuthorDate,
    }
}
