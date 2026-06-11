using System;

namespace Shift.Contract
{
    public class DeleteCaseDocumentRequest
    {
        public Guid CaseId { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}