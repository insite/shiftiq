using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionHotspotImageChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public HotspotImage Image { get; }

        public QuestionHotspotImageChanged(Guid questionIdentifier, HotspotImage image)
        {
            QuestionIdentifier = questionIdentifier;
            Image = image;
        }
    }
}
