using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ModifyMandatorySurvey : Command
    {
        public Guid? SurveyForm { get; set; }

        public ModifyMandatorySurvey(Guid @event, Guid? surveyForm)
        {
            AggregateIdentifier = @event;
            SurveyForm = surveyForm;
        }
    }
}
