using System;

namespace Shift.Contract
{
    public partial class SubmissionAnswerMatch
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}