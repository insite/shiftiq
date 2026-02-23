using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestionOrderingSolutionOptions : Command
    {
        public Guid Question { get; set; }
        public Guid Solution { get; set; }
        public Guid[] OptionsOrder { get; set; }

        public ReorderQuestionOrderingSolutionOptions(Guid bank, Guid question, Guid solution, Guid[] optionsOrder)
        {
            AggregateIdentifier = bank;
            Question = question;
            Solution = solution;
            OptionsOrder = optionsOrder;
        }
    }
}
