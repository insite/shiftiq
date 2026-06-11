using System;

namespace Shift.Contract
{
    public partial class SubmissionOptionMatch
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyOptionId { get; set; }
    }
}