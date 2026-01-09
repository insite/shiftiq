using System;

using InSite.Domain.Organizations.PerformanceReport;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AssessmentSettings
    {
        public bool DisableStrictQuestionCompetencySelection { get; set; }
        public bool EnableQuestionSubCompetencySelection { get; set; }
        public bool LockPublishedQuestions { get; set; }
        public bool AttemptGradingAssessor { get; set; }
        public bool RubricReGradeKeepInitialScores { get; set; }
        public bool ShowPersonNameToGradingAssessor { get; set; }
        public bool RequireAutoStart { get; set; }
        public ReportSettings PerformanceReport { get; set; }

        public AssessmentSettings()
        {
            PerformanceReport = new ReportSettings();
        }

        public bool IsShallowEqual(AssessmentSettings other)
        {
            return DisableStrictQuestionCompetencySelection == other.DisableStrictQuestionCompetencySelection
                && EnableQuestionSubCompetencySelection == other.EnableQuestionSubCompetencySelection
                && LockPublishedQuestions == other.LockPublishedQuestions
                && AttemptGradingAssessor == other.AttemptGradingAssessor
                && RubricReGradeKeepInitialScores == other.RubricReGradeKeepInitialScores
                && ShowPersonNameToGradingAssessor == other.ShowPersonNameToGradingAssessor
                && RequireAutoStart == other.RequireAutoStart;
        }
    }
}
