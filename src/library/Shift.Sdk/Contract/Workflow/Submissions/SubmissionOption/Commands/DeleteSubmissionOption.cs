using System;

namespace Shift.Contract
{
    public class DeleteSubmissionOption
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyOptionIdentifier { get; set; }
    }
}