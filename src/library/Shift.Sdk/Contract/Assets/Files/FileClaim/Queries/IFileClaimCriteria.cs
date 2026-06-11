using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileClaimCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? ClaimGrantedSince { get; set; }
        DateTimeOffset? ClaimGrantedBefore { get; set; }
    }
}
