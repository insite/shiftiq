using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSubmissionOption : Query<SubmissionOptionModel>
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyOptionId { get; set; }
    }
}