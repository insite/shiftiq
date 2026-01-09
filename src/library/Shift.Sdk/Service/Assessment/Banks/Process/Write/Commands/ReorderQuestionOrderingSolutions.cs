using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ReorderQuestionOrderingSolutions : Command
    {
        public Guid Question { get; set; }
        public Guid[] SolutionsOrder { get; set; }

        public ReorderQuestionOrderingSolutions(Guid bank, Guid question, Guid[] solutionsOrder)
        {
            AggregateIdentifier = bank;
            Question = question;
            SolutionsOrder = solutionsOrder;
        }
    }
}
