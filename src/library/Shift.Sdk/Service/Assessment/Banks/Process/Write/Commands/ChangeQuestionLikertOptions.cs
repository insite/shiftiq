using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionLikertOptions : Command
    {
        public Guid Question { get; set; }
        public LikertOption[] Options { get; set; }

        public ChangeQuestionLikertOptions(Guid bank, Guid question, LikertOption[] options)
        {
            AggregateIdentifier = bank;
            Question = question;
            Options = options;
        }

    }
}
