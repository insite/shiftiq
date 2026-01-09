using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionCondition : Command
    {
        public Guid Question { get; set; }
        public string Condition { get; set; }

        public ChangeQuestionCondition(Guid bank, Guid question, string condition)
        {
            AggregateIdentifier = bank;
            Question = question;
            Condition = condition;
        }
    }
}
