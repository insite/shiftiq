using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSubmissionOption : Query<bool>
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyOptionId { get; set; }
    }
}