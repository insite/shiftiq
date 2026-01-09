using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class QuestionHotspotShowShapesChanged : Change
    {
        public Guid QuestionIdentifier { get; set; }
        public bool ShowShapes { get; set; }

        public QuestionHotspotShowShapesChanged(Guid questionIdentifier, bool showShapes)
        {
            QuestionIdentifier = questionIdentifier;
            ShowShapes = showShapes;
        }
    }
}
