using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertSubmissionOption : Query<bool>
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyOptionIdentifier { get; set; }
    }
}