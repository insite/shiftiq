using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCaseDocument : Query<CaseDocumentModel>
    {
        public Guid AttachmentIdentifier { get; set; }
    }
}