using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricRenamed : Change
    {
        public string RubricTitle { get; set; }

        public RubricRenamed(string rubricTitle)
        {
            RubricTitle = rubricTitle;
        }
    }
}
