using System;

namespace Shift.Contract
{
    public partial class AttemptSolutionMatch
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SolutionIdentifier { get; set; }
    }
}