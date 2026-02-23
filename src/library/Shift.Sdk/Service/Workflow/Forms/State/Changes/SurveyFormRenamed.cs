using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Forms
{
    public class SurveyFormRenamed : Change
    {
        public SurveyFormRenamed(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}