using System;

namespace InSite.Application.Surveys.Read
{
    public class QResponseAnswer
    {
        public Guid RespondentUserIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public string ResponseAnswerText { get; set; }

        public virtual QResponseSession ResponseSession { get; set; }
        public virtual QSurveyQuestion SurveyQuestion { get; set; }
    }
}
