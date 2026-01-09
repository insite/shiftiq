using System;
using System.Collections.Generic;

using Shift.Constant;

namespace InSite.Application.Surveys.Read
{
    [Serializable]
    public class QResponseAnalysisFilter
    {
        #region Classes

        [Serializable]
        public class Answer
        {
            #region Properties

            public Guid QuestionIdentifier { get; set; }

            public ComparisonType ComparisonType { get; set; }

            public string AnswerText { get; set; }

            #endregion

            #region Construction

            private Answer()
            {

            }

            public Answer(Guid questionId, ComparisonType comparison, string answerText)
            {
                QuestionIdentifier = questionId;
                ComparisonType = comparison;
                AnswerText = answerText;
            }

            #endregion
        }

        #endregion

        #region Properties

        public Guid? SurveyFormIdentifier { get; set; }

        public bool ShowSelections { get; set; }

        public bool ShowNumbers { get; set; }

        public bool ShowComments { get; set; }

        public bool ShowText { get; set; }

        public bool EnableQuestionFilter { get; set; }

        public List<Answer> AnswerFilter { get; }

        public string AnswerFilterOperator { get; set; }

        public Guid? GroupIdentifier { get; set; }

        public Guid OrganizationIdentifier { get; set; }
        public Guid? SurveyQuestionIdentifier { get; set; }
        public string ResponseAnswerText { get; set; }
        public string RespondentName { get; set; }
        public DateTimeOffset? StartedSince { get; set; }
        public DateTimeOffset? StartedBefore { get; set; }
        public DateTimeOffset? CompletedSince { get; set; }
        public DateTimeOffset? CompletedBefore { get; set; }
        public bool IsPlatformAdministrator { get; set; }
        public bool? IsLocked { get; set; }

        #endregion

        #region Construction

        public QResponseAnalysisFilter()
        {
            ShowSelections = true;
            ShowNumbers = true;
            ShowComments = true;
            ShowText = true;
            EnableQuestionFilter = false;

            AnswerFilter = new List<Answer>();
        }

        #endregion
    }
}
