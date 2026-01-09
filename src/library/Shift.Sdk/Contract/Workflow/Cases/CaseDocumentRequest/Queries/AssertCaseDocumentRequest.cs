using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCaseDocumentRequest : Query<bool>
    {
        public Guid CaseIdentifier { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}