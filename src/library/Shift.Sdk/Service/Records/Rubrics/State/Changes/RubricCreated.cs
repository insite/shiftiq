using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class RubricCreated : Change
    {
        public string RubricTitle { get; set; }

        public RubricCreated(string rubricTitle)
        {
            RubricTitle = rubricTitle;
        }
    }
}
