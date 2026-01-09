using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchCaseDocuments : Query<IEnumerable<CaseDocumentMatch>>, ICaseDocumentCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? CaseIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PosterIdentifier { get; set; }

        public string FileName { get; set; }
        public string FileType { get; set; }

        public DateTimeOffset? AttachmentPosted { get; set; }
    }
}