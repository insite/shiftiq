using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionHotspotPinLimitChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public int PinLimit { get; set; }

        public QuestionHotspotPinLimitChanged(Guid questionIdentifier, int pinLimit)
        {
            QuestionIdentifier = questionIdentifier;
            PinLimit = pinLimit;
        }
    }
}
