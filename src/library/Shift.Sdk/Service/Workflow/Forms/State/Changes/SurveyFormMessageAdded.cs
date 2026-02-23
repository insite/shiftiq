using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormMessageAdded : Change
    {
        public SurveyFormMessageAdded(SurveyMessage message)
        {
            Message = message;
        }

        public SurveyMessage Message { get; }
    }
}
