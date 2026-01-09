using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionFlag : Command
    {
        public Guid Question { get; set; }
        public FlagType Flag { get; set; }

        public ChangeQuestionFlag(Guid bank, Guid question, FlagType flag)
        {
            AggregateIdentifier = bank;
            Question = question;
            Flag = flag;
        }
    }
}
