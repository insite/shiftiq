using System;

namespace Shift.Contract
{
    public partial class CaseDocumentRequestMatch
    {
        public Guid CaseId { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}