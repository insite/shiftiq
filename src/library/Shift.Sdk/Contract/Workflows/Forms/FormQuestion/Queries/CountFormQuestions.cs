using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFormQuestions : Query<int>, IFormQuestionCriteria
    {
        public Guid? OrganizationId { get; set; }
    }
}
