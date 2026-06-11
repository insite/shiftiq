using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionSet: Command
    {
        public Guid Question { get; set; }
        public Guid Set { get; set; }

        public ChangeQuestionSet(Guid bank, Guid question, Guid set)
        {
            AggregateIdentifier = bank;
            Question = question;
            Set = set;
        }
    }
}
