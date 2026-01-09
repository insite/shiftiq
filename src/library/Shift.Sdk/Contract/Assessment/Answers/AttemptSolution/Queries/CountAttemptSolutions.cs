using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttemptSolutions : Query<int>, IAttemptSolutionCriteria
    {
        public bool? AnswerIsMatched { get; set; }

        public string SolutionOptionsOrder { get; set; }

        public int? QuestionSequence { get; set; }
        public int? SolutionSequence { get; set; }

        public decimal? SolutionCutScore { get; set; }
        public decimal? SolutionPoints { get; set; }
    }
}