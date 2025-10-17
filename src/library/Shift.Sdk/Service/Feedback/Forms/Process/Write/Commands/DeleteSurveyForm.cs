using System;

using Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyForm : Command
    {
        public DeleteSurveyForm(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }
}