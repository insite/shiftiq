using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteQuestionHotspotOption : Command
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }

        public DeleteQuestionHotspotOption(Guid bank, Guid question, Guid option)
        {
            AggregateIdentifier = bank;
            Question = question;
            Option = option;
        }
    }
}
