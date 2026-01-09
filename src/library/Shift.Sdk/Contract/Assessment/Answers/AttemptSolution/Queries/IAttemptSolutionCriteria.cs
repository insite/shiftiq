using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptSolutionCriteria
    {
        QueryFilter Filter { get; set; }
        
        bool? AnswerIsMatched { get; set; }

        string SolutionOptionsOrder { get; set; }

        int? QuestionSequence { get; set; }
        int? SolutionSequence { get; set; }

        decimal? SolutionCutScore { get; set; }
        decimal? SolutionPoints { get; set; }
    }
}