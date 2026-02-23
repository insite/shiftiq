using System;

namespace Shift.Contract
{
    public partial class SubmissionAnswerMatch
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyQuestionId { get; set; }
    }
}