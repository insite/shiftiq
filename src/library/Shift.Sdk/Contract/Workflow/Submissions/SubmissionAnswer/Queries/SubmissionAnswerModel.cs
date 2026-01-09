using System;

namespace Shift.Contract
{
    public partial class SubmissionAnswerModel
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid RespondentUserIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }

        public string ResponseAnswerText { get; set; }
    }
}