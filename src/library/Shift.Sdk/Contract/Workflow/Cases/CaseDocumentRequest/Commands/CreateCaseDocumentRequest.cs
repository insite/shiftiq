using System;

namespace Shift.Contract
{
    public class CreateCaseDocumentRequest
    {
        public Guid CaseIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RequestedUserIdentifier { get; set; }

        public string RequestedFileCategory { get; set; }
        public string RequestedFileDescription { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }

        public DateTimeOffset RequestedTime { get; set; }
    }
}