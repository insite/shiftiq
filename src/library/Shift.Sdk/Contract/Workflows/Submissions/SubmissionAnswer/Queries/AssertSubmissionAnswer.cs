using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSubmissionAnswer : Query<bool>
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyQuestionId { get; set; }
    }
}