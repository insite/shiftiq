using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionMatchesChanged : Change
    {
        public Guid Question { get; set; }
        public MatchingList Matches { get; set; }

        public QuestionMatchesChanged(Guid question, MatchingList matches)
        {
            Question = question;
            Matches = matches;
        }
    }
}
