using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountFileClaims : Query<int>, IFileClaimCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? ClaimGrantedSince { get; set; }
        public DateTimeOffset? ClaimGrantedBefore { get; set; }
    }
}
