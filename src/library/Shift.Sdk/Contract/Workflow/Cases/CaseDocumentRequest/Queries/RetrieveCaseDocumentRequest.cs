using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCaseDocumentRequest : Query<CaseDocumentRequestModel>
    {
        public Guid CaseIdentifier { get; set; }

        public string RequestedFileCategory { get; set; }
    }
}