using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyWorkflowConfigured : Change
    {
        public SurveyWorkflowConfigured(SurveyWorkflowConfiguration configuration)
        {
            Configuration = configuration;
        }

        public SurveyWorkflowConfiguration Configuration { get; }
    }
}