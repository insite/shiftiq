using System;

namespace Shift.Contract
{
    public partial class SubmissionAnswerModel
    {
        public Guid? OrganizationId { get; set; }
        public Guid RespondentUserId { get; set; }
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyQuestionId { get; set; }

        public string ResponseAnswerText { get; set; }
    }
}