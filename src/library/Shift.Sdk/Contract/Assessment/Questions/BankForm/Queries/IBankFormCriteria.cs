using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IBankFormCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? BankIdentifier { get; set; }
        Guid? GradebookIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? SpecIdentifier { get; set; }
        Guid? WhenAttemptCompletedNotifyAdminMessageIdentifier { get; set; }
        Guid? WhenAttemptStartedNotifyAdminMessageIdentifier { get; set; }

        bool? FormHasDiagrams { get; set; }
        bool? FormThirdPartyAssessmentIsEnabled { get; set; }

        string BankLevelType { get; set; }
        string FormClassificationInstrument { get; set; }
        string FormCode { get; set; }
        string FormHasReferenceMaterials { get; set; }
        string FormHook { get; set; }
        string FormInstructionsForOnline { get; set; }
        string FormInstructionsForPaper { get; set; }
        string FormIntroduction { get; set; }
        string FormMaterialsForDistribution { get; set; }
        string FormMaterialsForParticipation { get; set; }
        string FormName { get; set; }
        string FormOrigin { get; set; }
        string FormPublicationStatus { get; set; }
        string FormSource { get; set; }
        string FormSummary { get; set; }
        string FormTitle { get; set; }
        string FormType { get; set; }
        string FormVersion { get; set; }

        int? AttemptGradedCount { get; set; }
        int? AttemptPassedCount { get; set; }
        int? AttemptStartedCount { get; set; }
        int? AttemptSubmittedCount { get; set; }
        int? FieldCount { get; set; }
        int? FormAsset { get; set; }
        int? FormAssetVersion { get; set; }
        int? FormAttemptLimit { get; set; }
        int? FormTimeLimit { get; set; }
        int? SectionCount { get; set; }
        int? SpecQuestionLimit { get; set; }

        decimal? FormPassingScore { get; set; }

        DateTimeOffset? FormFirstPublished { get; set; }
    }
}