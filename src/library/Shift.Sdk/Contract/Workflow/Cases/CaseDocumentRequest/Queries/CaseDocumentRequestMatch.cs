using System;

namespace Shift.Contract
{
    public partial class CaseDocumentRequestMatch
    {
        public Guid CaseIdentifier { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}