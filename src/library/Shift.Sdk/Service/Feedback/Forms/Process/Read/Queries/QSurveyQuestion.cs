using System;
using System.Collections.Generic;

namespace InSite.Application.Surveys.Read
{
    public class QSurveyQuestion
    {
        public Guid SurveyFormIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public string SurveyQuestionAttribute { get; set; }
        public string SurveyQuestionCode { get; set; }
        public string SurveyQuestionIndicator { get; set; }
        public string SurveyQuestionSource { get; set; }
        public string SurveyQuestionType { get; set; }
        
        public bool SurveyQuestionIsRequired { get; set; }
        public bool SurveyQuestionIsNested { get; set; }
        public bool SurveyQuestionListEnableBranch { get; set; }
        public bool SurveyQuestionListEnableGroupMembership { get; set; }
        public bool SurveyQuestionListEnableOtherText { get; set; }
        public bool SurveyQuestionListEnableRandomization { get; set; }
        public bool SurveyQuestionNumberEnableStatistics { get; set; }

        public int SurveyQuestionSequence { get; set; }
        public int? SurveyQuestionTextCharacterLimit { get; set; }
        public int? SurveyQuestionTextLineCount { get; set; }

        public virtual QSurveyForm SurveyForm { get; set; }

        public virtual ICollection<QSurveyOptionItem> BranchFromOptions { get; set; }
        public virtual ICollection<QSurveyCondition> QSurveyConditions { get; set; }
        public virtual ICollection<QSurveyOptionList> QSurveyOptionLists { get; set; }
        public virtual ICollection<QResponseAnswer> QResponseAnswers { get; set; }

        public QSurveyQuestion()
        {
            BranchFromOptions = new HashSet<QSurveyOptionItem>();
            QSurveyConditions = new HashSet<QSurveyCondition>();
            QSurveyOptionLists = new HashSet<QSurveyOptionList>();
            QResponseAnswers = new HashSet<QResponseAnswer>();
        }
    }
}
