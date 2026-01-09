using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSubmissionOption : Query<SubmissionOptionModel>
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyOptionIdentifier { get; set; }
    }
}