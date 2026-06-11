using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionHotspotOptionAdded : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid OptionIdentifier { get; set; }
        public HotspotShape Shape { get; }
        public ContentTitle Content { get; }
        public decimal Points { get; }

        public QuestionHotspotOptionAdded(Guid questionIdentifier, Guid optionIdentifier, HotspotShape shape, ContentTitle content, decimal points)
        {
            QuestionIdentifier = questionIdentifier;
            OptionIdentifier = optionIdentifier;
            Shape = shape;
            Content = content;
            Points = points;
        }
    }
}
