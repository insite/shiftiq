using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionTrigger : Command
    {
        public Guid Question { get; set; }
        public int Index { get; set; }

        public DeleteQuestionTrigger(Guid bank, Guid question, int index)
        {
            AggregateIdentifier = bank;
            Question = question;
            Index = index;
        }
    }
}
