using System;

using Common.Timeline.Commands;

namespace InSite.Application.Surveys.Write
{
    public class AddSurveyRespondents : Command
    {
        public AddSurveyRespondents(Guid form, Guid[] respondents)
        {
            AggregateIdentifier = form;
            Respondents = respondents;
        }

        public Guid[] Respondents { get; set; }
    }
}