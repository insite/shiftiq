using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionItemContentChanged : Change
    {
        public SurveyOptionItemContentChanged(Guid item, ContentContainer content)
        {
            Item = item;
            Content = content;
        }

        public Guid Item { get; }
        public ContentContainer Content { get; }
    }
}