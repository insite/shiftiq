using System;

using InSite.Domain.Organizations.PerformanceReport;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class AssessmentSettings
    {
        [Serializable]
        public class TakerReportSettings
        {
            public string DocumentType { get; set; }
            public string DocumentSubtype { get; set; }
        }

        public bool DisableStrictQuestionCompetencySelection { get; set; }
        public bool EnableQuestionSubCompetencySelection { get; set; }
        public bool LockPublishedQuestions { get; set; }
        public bool AttemptGradingAssessor { get; set; }
        public bool RubricReGradeKeepInitialScores { get; set; }
        public bool ShowPersonNameToGradingAssessor { get; set; }
        public bool RequireAutoStart { get; set; }
        public bool LockPublishedStandards { get; set; }
        public ReportSettings PerformanceReport { get; set; }
        public TakerReportSettings TakerReport { get; set; }

        public AssessmentSettings()
        {
            PerformanceReport = new ReportSettings();
            TakerReport = new TakerReportSettings();
        }

        public bool IsShallowEqual(AssessmentSettings other)
        {
            return DisableStrictQuestionCompetencySelection == other.DisableStrictQuestionCompetencySelection
                && EnableQuestionSubCompetencySelection == other.EnableQuestionSubCompetencySelection
                && LockPublishedQuestions == other.LockPublishedQuestions
                && AttemptGradingAssessor == other.AttemptGradingAssessor
                && RubricReGradeKeepInitialScores == other.RubricReGradeKeepInitialScores
                && ShowPersonNameToGradingAssessor == other.ShowPersonNameToGradingAssessor
                && RequireAutoStart == other.RequireAutoStart
                && LockPublishedStandards == other.LockPublishedStandards
                && TakerReport?.DocumentType == other.TakerReport?.DocumentType
                && TakerReport?.DocumentSubtype == other.TakerReport?.DocumentSubtype
                ;
        }
    }
}
