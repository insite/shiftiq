using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseDescribed : Change
    {
        public string Description { get; set; }

        public CaseDescribed(string description)
        {
            Description = description;
        }
    }
}
