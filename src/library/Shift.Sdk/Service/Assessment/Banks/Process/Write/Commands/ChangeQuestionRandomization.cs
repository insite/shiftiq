using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionRandomization : Command
    {
        public Guid Question { get; set; }
        public Randomization Randomization { get; set; }

        public ChangeQuestionRandomization(Guid bank, Guid question, Randomization randomization)
        {
            AggregateIdentifier = bank;
            Question = question;
            Randomization = randomization;
        }
    }
}
