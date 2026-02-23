using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestionOrderingOptions : Command
    {
        public Guid Question { get; set; }
        public Guid[] OptionsOrder { get; set; }

        public ReorderQuestionOrderingOptions(Guid bank, Guid question, Guid[] optionsOrder)
        {
            AggregateIdentifier = bank;
            Question = question;
            OptionsOrder = optionsOrder;
        }
    }
}
