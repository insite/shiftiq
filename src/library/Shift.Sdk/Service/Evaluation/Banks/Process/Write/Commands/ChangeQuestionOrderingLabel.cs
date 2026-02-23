using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionOrderingLabel : Command
    {
        public Guid Question { get; set; }
        public OrderingLabel Label { get; set; }

        public ChangeQuestionOrderingLabel(Guid bank, Guid question, OrderingLabel label)
        {
            AggregateIdentifier = bank;
            Question = question;
            Label = label;
        }
    }
}
