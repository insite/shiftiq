using System.ComponentModel;

namespace Shift.Constant
{
    public enum PrerequisiteType
    {
        [Description("None")]
        None,

        [Description("Activity Completed")]
        ActivityCompleted,

        [Description("Assessment Passed")]
        AssessmentPassed,

        [Description("Assessment Failed")]
        AssessmentFailed,

        [Description("Assessment Scored")]
        AssessmentScored,

        [Description("Question Answered Correctly")]
        QuestionAnsweredCorrectly,

        [Description("Question Answered Incorrectly")]
        QuestionAnsweredIncorrectly,

        [Description("Grade Item Passed")]
        GradeItemPassed,

        [Description("Grade Item Failed")]
        GradeItemFailed,

        [Description("Task Completed")]
        TaskCompleted,

        [Description("Task Viewed")]
        TaskViewed
    }
}
