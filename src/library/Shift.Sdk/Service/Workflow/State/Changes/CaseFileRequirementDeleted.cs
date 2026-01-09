using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseFileRequirementDeleted : Change
    {
        public string RequestedFileCategory { get; set; }

        public CaseFileRequirementDeleted(string requestedFileCategory)
        {
            RequestedFileCategory = requestedFileCategory;
        }
    }
}
