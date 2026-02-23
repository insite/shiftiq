using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormStatusChanged : Change
    {
        public SurveyFormStatusChanged(SurveyFormStatus status)
        {
            Status = status;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public SurveyFormStatus Status { get; set; }
    }
}