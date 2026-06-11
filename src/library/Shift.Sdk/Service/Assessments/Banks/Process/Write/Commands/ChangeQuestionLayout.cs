using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionLayout : Command
    {
        public Guid Question { get; set; }
        public OptionLayout Layout { get; set; }

        public ChangeQuestionLayout(Guid bank, Guid question, OptionLayout layout)
        {
            AggregateIdentifier = bank;
            Question = question;
            Layout = layout;
        }
    }
}
