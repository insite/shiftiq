using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DisconnectQuestionRubric : Command
    {
        public Guid Question { get; set; }

        public DisconnectQuestionRubric(Guid bank, Guid question)
        {
            AggregateIdentifier = bank;
            Question = question;
        }
    }
}
