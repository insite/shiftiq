using System;

namespace Shift.Contract
{
    public class DeleteSubmissionAnswer
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}