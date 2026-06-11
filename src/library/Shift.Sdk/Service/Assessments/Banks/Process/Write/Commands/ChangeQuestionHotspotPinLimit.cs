using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionHotspotPinLimit : Command
    {
        public Guid Question { get; set; }
        public int PinLimit { get; set; }

        public ChangeQuestionHotspotPinLimit(Guid bank, Guid question, int pinLimit)
        {
            AggregateIdentifier = bank;
            Question = question;
            PinLimit = pinLimit;
        }
    }
}
