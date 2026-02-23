using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAssessments : Query<int>, IAssessmentCriteria
    {
        public Guid? BankId { get; set; }
        public Guid? GradebookId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? SpecId { get; set; }
        public Guid? WhenAttemptCompletedNotifyAdminMessageId { get; set; }
        public Guid? WhenAttemptStartedNotifyAdminMessageId { get; set; }

        public bool? FormHasDiagrams { get; set; }
        public bool? FormThirdPartyAssessmentIsEnabled { get; set; }

        public string BankLevelType { get; set; }
        public string FormClassificationInstrument { get; set; }
        public string FormCode { get; set; }
        public string FormHasReferenceMaterials { get; set; }
        public string FormHook { get; set; }
        public string FormInstructionsForOnline { get; set; }
        public string FormInstructionsForPaper { get; set; }
        public string FormIntroduction { get; set; }
        public string FormMaterialsForDistribution { get; set; }
        public string FormMaterialsForParticipation { get; set; }
        public string FormName { get; set; }
        public string FormOrigin { get; set; }
        public string FormPublicationStatus { get; set; }
        public string FormSource { get; set; }
        public string FormSummary { get; set; }
        public string FormTitle { get; set; }
        public string FormType { get; set; }
        public string FormVersion { get; set; }

        public int? AttemptGradedCount { get; set; }
        public int? AttemptPassedCount { get; set; }
        public int? AttemptStartedCount { get; set; }
        public int? AttemptSubmittedCount { get; set; }
        public int? FieldCount { get; set; }
        public int? FormAsset { get; set; }
        public int? FormAssetVersion { get; set; }
        public int? FormAttemptLimit { get; set; }
        public int? FormTimeLimit { get; set; }
        public int? SectionCount { get; set; }
        public int? SpecQuestionLimit { get; set; }

        public decimal? FormPassingScore { get; set; }

        public DateTimeOffset? FormFirstPublished { get; set; }
    }
}