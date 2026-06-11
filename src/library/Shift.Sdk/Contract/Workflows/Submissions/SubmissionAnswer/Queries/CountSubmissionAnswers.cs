using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountSubmissionAnswers : Query<int>, ISubmissionAnswerCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
