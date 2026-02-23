using System;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptSolution
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public int QuestionSequence { get; set; }
        public Guid SolutionIdentifier { get; set; }
        public int SolutionSequence { get; set; }
        public string SolutionOptionsOrder { get; set; }
        public decimal SolutionPoints { get; set; }
        public decimal? SolutionCutScore { get; set; }
        public bool AnswerIsMatched { get; set; }

        public virtual QAttempt Attempt { get; set; }
    }
}
