using System.ComponentModel;

namespace Shift.Constant
{
    public enum JournalSetupFieldType
    {
        [Description("Employer")]
        Employer,

        [Description("Supervisor")]
        Supervisor,

        [Description("Start and End Dates")]
        StartAndEndDates,

        [Description("Date Completed")]
        Completed,

        [Description("Hours")]
        Hours,

        [Description("Training Evidence")]
        TrainingEvidence,

        [Description("Audio and Video Evidence")]
        MediaEvidence,

        [Description("Apprenticeship Training Level")]
        TrainingLevel,

        [Description("Training Location (Employer name or Training provider)")]
        TrainingLocation,

        [Description("Training Provider")]
        TrainingProvider,

        [Description("Course Title")]
        TrainingCourseTitle,

        [Description("Comment")]
        TrainingComment,

        [Description("Type of Training")]
        TrainingType,
    }
}