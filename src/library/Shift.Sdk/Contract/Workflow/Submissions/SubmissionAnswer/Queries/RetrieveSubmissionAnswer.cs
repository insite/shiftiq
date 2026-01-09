using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSubmissionAnswer : Query<SubmissionAnswerModel>
    {
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyQuestionIdentifier { get; set; }
    }
}