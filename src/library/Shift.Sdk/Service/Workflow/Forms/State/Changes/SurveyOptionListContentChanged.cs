using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyOptionListContentChanged : Change
    {
        public SurveyOptionListContentChanged(Guid list, ContentContainer content, string category)
        {
            List = list;
            Content = content;
            Category = category;
        }

        public Guid List { get; }
        public ContentContainer Content { get; }
        public string Category { get; }
    }
}