using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class CaseFileRequirementCompleted : Change
    {
        public string RequestedFileCategory { get; set; }
        public string FileName { get; set; }
        public Guid FileIdentifier { get; set; }

        public CaseFileRequirementCompleted(string requestedFileCategory, string fileName, Guid fileIdentifier)
        {
            RequestedFileCategory = requestedFileCategory;
            FileName = fileName;
            FileIdentifier = fileIdentifier;
        }
    }
}
