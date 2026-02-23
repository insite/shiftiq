using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseFileRequirementAdded : Change
    {
        public string RequestedFrom { get; set; }
        public string RequestedFileCategory { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFileDescription { get; set; }

        public CaseFileRequirementAdded(
            string requestedFrom,
            string requestedFileCategory,
            string requestedFileSubcategory,
            string requestedFileDescription
            )
        {
            RequestedFileCategory = requestedFileCategory;
            RequestedFrom = requestedFrom;
            RequestedFileCategory = requestedFileCategory;
            RequestedFileSubcategory = requestedFileSubcategory;
            RequestedFileDescription = requestedFileDescription;
        }
    }
}
