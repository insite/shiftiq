using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormMessages : Command
    {
        public ChangeSurveyFormMessages(Guid form, SurveyMessage[] messages)
        {
            AggregateIdentifier = form;
            Messages = messages;
        }

        public SurveyMessage[] Messages { get; }
    }
}