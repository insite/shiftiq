using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormContent : Command
    {
        public ChangeSurveyFormContent(Guid form, ContentContainer content)
        {
            AggregateIdentifier = form;
            Content = content;
        }

        public ContentContainer Content { get; }
    }
}