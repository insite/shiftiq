using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricDescribed : Change
    {
        public string RubricDescription { get; set; }

        public RubricDescribed(string rubricDescription)
        {
            RubricDescription = rubricDescription;
        }
    }
}
