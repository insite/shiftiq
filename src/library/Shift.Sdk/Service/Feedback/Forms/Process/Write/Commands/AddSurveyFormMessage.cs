using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyFormMessage : Command
    {
        public AddSurveyFormMessage(Guid form, SurveyMessage message)
        {
            AggregateIdentifier = form;
            Message = message;
        }

        public SurveyMessage Message { get; }
    }
}
