using System;

using Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class DeleteSurveyRespondents : Command
    {
        public DeleteSurveyRespondents(Guid form, Guid[] respondents)
        {
            AggregateIdentifier = form;
            Respondents = respondents;
        }

        public Guid[] Respondents { get; set; }
    }
}