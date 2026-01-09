using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFormQuestions : Query<IEnumerable<FormQuestionMatch>>, IFormQuestionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }

        public bool? SurveyQuestionIsNested { get; set; }
        public bool? SurveyQuestionIsRequired { get; set; }
        public bool? SurveyQuestionListEnableBranch { get; set; }
        public bool? SurveyQuestionListEnableGroupMembership { get; set; }
        public bool? SurveyQuestionListEnableOtherText { get; set; }
        public bool? SurveyQuestionListEnableRandomization { get; set; }
        public bool? SurveyQuestionNumberEnableStatistics { get; set; }

        public string SurveyQuestionAttribute { get; set; }
        public string SurveyQuestionCode { get; set; }
        public string SurveyQuestionIndicator { get; set; }
        public string SurveyQuestionSource { get; set; }
        public string SurveyQuestionType { get; set; }

        public int? SurveyQuestionSequence { get; set; }
        public int? SurveyQuestionTextCharacterLimit { get; set; }
        public int? SurveyQuestionTextLineCount { get; set; }
    }
}