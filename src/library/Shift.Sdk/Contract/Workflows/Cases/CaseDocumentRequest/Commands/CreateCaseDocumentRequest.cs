using System;

namespace Shift.Contract
{
    public class CreateCaseDocumentRequest
    {
        public Guid CaseId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid RequestedUserId { get; set; }

        public string RequestedFileCategory { get; set; }
        public string RequestedFileDescription { get; set; }
        public string RequestedFileSubcategory { get; set; }
        public string RequestedFrom { get; set; }

        public DateTimeOffset RequestedTime { get; set; }
    }
}