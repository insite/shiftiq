using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFileClaims : Query<IEnumerable<FileClaimMatch>>, IFileClaimCriteria
    {
        public Guid? FileIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }

        public string ObjectType { get; set; }

        public DateTimeOffset? ClaimGranted { get; set; }
    }
}