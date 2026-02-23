using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class QuestionHotspotOptionChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public Guid OptionIdentifier { get; set; }
        public HotspotShape Shape { get; set; }
        public ContentTitle Content { get; set; }
        public decimal Points { get; set; }

        public QuestionHotspotOptionChanged(Guid questionIdentifier, Guid optionIdentifier, HotspotShape shape, ContentTitle content, decimal points)
        {
            QuestionIdentifier = questionIdentifier;
            OptionIdentifier = optionIdentifier;
            Shape = shape;
            Content = content;
            Points = points;
        }
    }
}
