using System;

namespace Shift.Contract
{
    public class DeleteSubmissionOption
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyOptionId { get; set; }
    }
}