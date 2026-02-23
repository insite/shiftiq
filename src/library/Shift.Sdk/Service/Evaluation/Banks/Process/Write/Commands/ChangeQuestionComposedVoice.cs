using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionComposedVoice : Command
    {
        public Guid Question { get; set; }
        public ComposedVoice ComposedVoice { get; set; }
        public int TimeLimit { get; set; }
        public int AttemptLimit { get; set; }

        public ChangeQuestionComposedVoice(Guid bank, Guid question, ComposedVoice composedVoice)
        {
            AggregateIdentifier = bank;
            Question = question;
            ComposedVoice = composedVoice;
        }
    }
}
