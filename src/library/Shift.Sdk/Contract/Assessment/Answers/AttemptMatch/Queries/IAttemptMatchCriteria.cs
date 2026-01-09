using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IAttemptMatchCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }

        string AnswerText { get; set; }
        string MatchLeftText { get; set; }
        string MatchRightText { get; set; }

        int? QuestionSequence { get; set; }

        decimal? MatchPoints { get; set; }
    }
}