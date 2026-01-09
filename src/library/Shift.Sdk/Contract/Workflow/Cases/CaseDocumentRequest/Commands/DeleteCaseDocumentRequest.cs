using System;

namespace Shift.Contract
{
    public class DeleteCaseDocumentRequest
    {
        public Guid CaseIdentifier { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}