using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveSubmissionAnswer : Query<SubmissionAnswerModel>
    {
        public Guid ResponseSessionId { get; set; }
        public Guid SurveyQuestionId { get; set; }
    }
}