using System;

namespace Shift.Contract
{
    public class ModifyAttemptSolution
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public Guid SolutionIdentifier { get; set; }

        public bool? AnswerIsMatched { get; set; }

        public string SolutionOptionsOrder { get; set; }

        public int QuestionSequence { get; set; }
        public int SolutionSequence { get; set; }

        public decimal? SolutionCutScore { get; set; }
        public decimal SolutionPoints { get; set; }
    }
}