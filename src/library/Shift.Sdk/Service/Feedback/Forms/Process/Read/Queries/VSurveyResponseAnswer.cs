using System;

namespace InSite.Application.Surveys.Read
{
    public class VSurveyResponseAnswer
    {
        public Guid RandomIdentifier { get; set; }

        public Guid ResponseSessionIdentifier { get; set; }
        public Guid? SurveyOptionItemIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string AnswerText { get; set; }
        public string OptionText { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }

        public DateTimeOffset? ResponseSessionStarted { get; set; }
        public DateTimeOffset? ResponseSessionCompleted { get; set; }
    }
}
