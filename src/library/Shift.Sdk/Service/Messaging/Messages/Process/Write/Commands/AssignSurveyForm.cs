using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Messages.Write
{
    public class AssignSurveyForm : Command
    {
        public Guid SurveyFormIdentifier { get; set; }

        public AssignSurveyForm(Guid message, Guid survey)
        {
            AggregateIdentifier = message;
            SurveyFormIdentifier = survey;
        }
    }
}