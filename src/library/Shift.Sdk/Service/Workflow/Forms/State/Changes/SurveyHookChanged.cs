using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyHookChanged : Change
    {
        public SurveyHookChanged(string hook)
        {
            Hook = hook;
        }

        public string Hook { get; set; }
    }
}