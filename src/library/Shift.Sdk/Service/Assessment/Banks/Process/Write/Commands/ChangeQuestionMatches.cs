using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionMatches : Command
    {
        public Guid Question { get; set; }
        public MatchingList Matches { get; set; }

        public ChangeQuestionMatches(Guid bank, Guid question, MatchingList matches)
        {
            AggregateIdentifier = bank;
            Question = question;
            Matches = matches;
        }
    }
}
