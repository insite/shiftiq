using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountForms : Query<int>, IFormCriteria
    {
        public Guid? LastChangeUser { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyMessageInvitation { get; set; }
        public Guid? SurveyMessageResponseCompleted { get; set; }
        public Guid? SurveyMessageResponseConfirmed { get; set; }
        public Guid? SurveyMessageResponseStarted { get; set; }

        public bool? DisplaySummaryChart { get; set; }
        public bool? EnableUserConfidentiality { get; set; }
        public bool? HasWorkflowConfiguration { get; set; }
        public bool? RequireUserAuthentication { get; set; }
        public bool? RequireUserIdentification { get; set; }

        public string LastChangeType { get; set; }
        public string SurveyFormHook { get; set; }
        public string SurveyFormLanguage { get; set; }
        public string SurveyFormLanguageTranslations { get; set; }
        public string SurveyFormName { get; set; }
        public string SurveyFormStatus { get; set; }
        public string UserFeedback { get; set; }

        public int? AssetNumber { get; set; }
        public int? BranchCount { get; set; }
        public int? ConditionCount { get; set; }
        public int? PageCount { get; set; }
        public int? QuestionCount { get; set; }
        public int? ResponseLimitPerUser { get; set; }
        public int? SurveyFormDurationMinutes { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public DateTimeOffset? SurveyFormClosed { get; set; }
        public DateTimeOffset? SurveyFormLocked { get; set; }
        public DateTimeOffset? SurveyFormOpened { get; set; }
    }
}