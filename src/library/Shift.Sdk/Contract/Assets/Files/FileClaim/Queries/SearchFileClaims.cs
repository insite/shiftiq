using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchFileClaims : Query<IEnumerable<FileClaimMatch>>, IFileClaimCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ClaimGrantedSince { get; set; }
        public DateTimeOffset? ClaimGrantedBefore { get; set; }
    }
}
