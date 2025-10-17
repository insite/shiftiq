﻿using Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseFileRequirementModified : Change
    {
        public string RequestedFileCategory { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }
        public string RequestedFileDescription { get; set; }

        public CaseFileRequirementModified(
            string requestedFileCategory,
            string requestedFileSubcategory,
            string requestedFrom,
            string requestedFileDescription
            )
        {
            RequestedFrom = requestedFrom;
            RequestedFileCategory = requestedFileCategory;
            RequestedFileCategory = requestedFileCategory;
            RequestedFileSubcategory = requestedFileSubcategory;
            RequestedFileDescription = requestedFileDescription;
        }
    }
}
