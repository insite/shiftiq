using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionHotspotOptionDeleted : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid OptionIdentifier { get; set; }

        public QuestionHotspotOptionDeleted(Guid questionIdentifier, Guid optionIdentifier)
        {
            QuestionIdentifier = questionIdentifier;
            OptionIdentifier = optionIdentifier;
        }
    }
}
