using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveAttemptSolution : Query<AttemptSolutionModel>
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SolutionIdentifier { get; set; }
    }
}