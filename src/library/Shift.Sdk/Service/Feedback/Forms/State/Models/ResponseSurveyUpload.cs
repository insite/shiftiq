using System;

namespace InSite.Domain.Surveys.Forms
{
    public class ResponseSurveyUpload
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyFormIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier {get; set;}
        public Guid RespondentUserIdentifier { get; set; }
        public string ResponseAnswerText { get; set;}
        public string SurveyFormName { get; set;}
        public DateTimeOffset? ResponseSessionStarted { get; set; }
        public int SurveyQuestionSequence { get; set; }
    }
}
