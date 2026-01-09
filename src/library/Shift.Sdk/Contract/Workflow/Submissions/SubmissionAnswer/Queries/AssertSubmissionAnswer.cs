using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSubmissionAnswer : Query<bool>
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}