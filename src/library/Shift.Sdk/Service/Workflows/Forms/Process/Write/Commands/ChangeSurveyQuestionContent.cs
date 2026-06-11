using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyQuestionContent : Command
    {
        public ChangeSurveyQuestionContent(Guid form, Guid question, ContentContainer content)
        {
            AggregateIdentifier = form;
            Question = question;
            Content = content;
        }

        public Guid Question { get; }
        public ContentContainer Content { get; }
    }
}