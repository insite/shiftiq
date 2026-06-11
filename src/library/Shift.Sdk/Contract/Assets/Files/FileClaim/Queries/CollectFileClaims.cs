using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectFileClaims : Query<IEnumerable<FileClaimModel>>, IFileClaimCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ClaimGrantedSince { get; set; }
        public DateTimeOffset? ClaimGrantedBefore { get; set; }
    }
}
