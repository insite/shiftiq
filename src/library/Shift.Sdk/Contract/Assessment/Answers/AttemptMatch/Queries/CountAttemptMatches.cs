using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountAttemptMatches : Query<int>, IAttemptMatchCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }

        public string AnswerText { get; set; }
        public string MatchLeftText { get; set; }
        public string MatchRightText { get; set; }

        public int? QuestionSequence { get; set; }

        public decimal? MatchPoints { get; set; }
    }
}