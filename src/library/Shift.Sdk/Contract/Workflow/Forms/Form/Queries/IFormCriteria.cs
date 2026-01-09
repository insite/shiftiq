using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? LastChangeUser { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? SurveyMessageInvitation { get; set; }
        Guid? SurveyMessageResponseCompleted { get; set; }
        Guid? SurveyMessageResponseConfirmed { get; set; }
        Guid? SurveyMessageResponseStarted { get; set; }

        bool? DisplaySummaryChart { get; set; }
        bool? EnableUserConfidentiality { get; set; }
        bool? HasWorkflowConfiguration { get; set; }
        bool? RequireUserAuthentication { get; set; }
        bool? RequireUserIdentification { get; set; }

        string LastChangeType { get; set; }
        string SurveyFormHook { get; set; }
        string SurveyFormLanguage { get; set; }
        string SurveyFormLanguageTranslations { get; set; }
        string SurveyFormName { get; set; }
        string SurveyFormStatus { get; set; }
        string UserFeedback { get; set; }

        int? AssetNumber { get; set; }
        int? BranchCount { get; set; }
        int? ConditionCount { get; set; }
        int? PageCount { get; set; }
        int? QuestionCount { get; set; }
        int? ResponseLimitPerUser { get; set; }
        int? SurveyFormDurationMinutes { get; set; }

        DateTimeOffset? LastChangeTime { get; set; }
        DateTimeOffset? SurveyFormClosed { get; set; }
        DateTimeOffset? SurveyFormLocked { get; set; }
        DateTimeOffset? SurveyFormOpened { get; set; }
    }
}