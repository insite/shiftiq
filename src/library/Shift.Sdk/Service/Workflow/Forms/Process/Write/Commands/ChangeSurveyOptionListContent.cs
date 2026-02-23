using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyOptionListContent : Command
    {
        public ChangeSurveyOptionListContent(Guid form, Guid list, ContentContainer content, string category)
        {
            AggregateIdentifier = form;
            List = list;
            Content = content;
            Category = category;
        }

        public Guid List { get; }
        public ContentContainer Content { get; }
        public string Category { get; }
    }
}