using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionOrderingOption : Command
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }

        public DeleteQuestionOrderingOption(Guid bank, Guid question, Guid option)
        {
            AggregateIdentifier = bank;
            Question = question;
            Option = option;
        }
    }
}
