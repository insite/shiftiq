using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFiles : Query<IEnumerable<FileMatch>>, IFileCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }

        public string ObjectTypeExact { get; set; }
        public string ObjectIdentifierContains { get; set; }
        public string FileNameContains { get; set; }
        public string DocumentNameContains { get; set; }

        public DateTimeOffset? FileUploadedSince { get; set; }
        public DateTimeOffset? FileUploadedBefore { get; set; }

        public bool? HasClaims { get; set; }
    }
}