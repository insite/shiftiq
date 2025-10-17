using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Surveys.Read
{
    public class QSurveyRespondent
    {
        public Guid RespondentUserIdentifier { get; set; }
        public Guid SurveyFormIdentifier { get; set; }

        public virtual VUser Respondent { get; set; }
        public virtual QSurveyForm SurveyForm { get; set; }
    }
}
