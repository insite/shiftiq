using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class ModifyFileRequirement : Command
    {
        public string RequestedFileCategory { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }
        public string RequestedFileDescription { get; set; }
        public string RequestedFileStatus { get; set; }

        public ModifyFileRequirement(
            Guid issue,
            string requestedFileCategory,
            string requestedFileSubcategory,
            string requestedFrom,
            string requestedFileDescription,
            string requestedFileStatus
            )
        {
            AggregateIdentifier = issue;
            RequestedFileCategory = requestedFileCategory;
            RequestedFileSubcategory = requestedFileSubcategory;
            RequestedFrom = requestedFrom;
            RequestedFileDescription = requestedFileDescription;
            RequestedFileStatus = requestedFileStatus;
        }
    }
}
