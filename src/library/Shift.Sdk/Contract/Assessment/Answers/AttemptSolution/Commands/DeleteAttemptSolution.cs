using System;

namespace Shift.Contract
{
    public class DeleteAttemptSolution
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SolutionIdentifier { get; set; }
    }
}