using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write
{
    public class ConfigureSurveyWorkflow : Command
    {
        public ConfigureSurveyWorkflow(Guid survey, SurveyWorkflowConfiguration configuration)
        {
            AggregateIdentifier = survey;
            Configuration = configuration;
        }

        public SurveyWorkflowConfiguration Configuration { get; }
    }
}