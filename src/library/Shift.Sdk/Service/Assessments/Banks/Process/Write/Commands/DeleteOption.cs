using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteOption : Command
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public DeleteOption(Guid bank, Guid question, int option)
        {
            AggregateIdentifier = bank;
            Question = question;
            Option = option;
        }
    }
}
