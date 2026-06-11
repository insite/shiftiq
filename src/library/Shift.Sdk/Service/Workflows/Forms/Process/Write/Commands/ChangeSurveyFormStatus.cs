using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Surveys.Write
{
    public class ChangeSurveyFormStatus : Command
    {
        public ChangeSurveyFormStatus(Guid form, SurveyFormStatus status)
        {
            AggregateIdentifier = form;
            Status = status;
        }

        public SurveyFormStatus Status { get; set; }
    }
}