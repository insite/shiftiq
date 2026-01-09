using System;

namespace Shift.Contract
{
    public partial class SubmissionOptionMatch
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyOptionIdentifier { get; set; }
    }
}