using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormQuestionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? SurveyFormIdentifier { get; set; }

        bool? SurveyQuestionIsNested { get; set; }
        bool? SurveyQuestionIsRequired { get; set; }
        bool? SurveyQuestionListEnableBranch { get; set; }
        bool? SurveyQuestionListEnableGroupMembership { get; set; }
        bool? SurveyQuestionListEnableOtherText { get; set; }
        bool? SurveyQuestionListEnableRandomization { get; set; }
        bool? SurveyQuestionNumberEnableStatistics { get; set; }

        string SurveyQuestionAttribute { get; set; }
        string SurveyQuestionCode { get; set; }
        string SurveyQuestionIndicator { get; set; }
        string SurveyQuestionSource { get; set; }
        string SurveyQuestionType { get; set; }

        int? SurveyQuestionSequence { get; set; }
        int? SurveyQuestionTextCharacterLimit { get; set; }
        int? SurveyQuestionTextLineCount { get; set; }
    }
}