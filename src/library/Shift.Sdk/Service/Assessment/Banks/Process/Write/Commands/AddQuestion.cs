using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class AddQuestion : Command
    {
        public Guid Set { get; set; }
        public Question Question { get; set; }

        public AddQuestion(Guid bank, Guid set, Question question)
        {
            AggregateIdentifier = bank;
            Set = set;
            Question = question;
        }
    }
}