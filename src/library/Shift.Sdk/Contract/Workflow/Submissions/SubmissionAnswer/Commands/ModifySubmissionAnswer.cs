using System;

namespace Shift.Contract
{
    public class ModifySubmissionAnswer
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid RespondentUserIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public string ResponseAnswerText { get; set; }
    }
}