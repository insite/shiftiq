using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertAttemptSolution : Query<bool>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SolutionIdentifier { get; set; }
    }
}