using System;

namespace Shift.Contract
{
    public class DeleteSubmissionAnswer
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyQuestionId { get; set; }
    }
}