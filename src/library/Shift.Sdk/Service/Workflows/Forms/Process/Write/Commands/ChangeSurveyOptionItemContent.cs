using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyOptionItemContent : Command
    {
        public ChangeSurveyOptionItemContent(Guid form, Guid item, ContentContainer content)
        {
            AggregateIdentifier = form;
            Item = item;
            Content = content;
        }

        public Guid Item { get; }
        public ContentContainer Content { get; }
    }
}