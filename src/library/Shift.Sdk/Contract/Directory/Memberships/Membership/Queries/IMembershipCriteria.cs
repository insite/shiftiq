using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMembershipCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? GroupId { get; set; }
        Guid? ModifiedBy { get; set; }
        Guid? UserId { get; set; }

        string MembershipFunction { get; set; }

        DateTimeOffset? MembershipEffective { get; set; }
        DateTimeOffset? MembershipExpiry { get; set; }
        DateTimeOffset? Modified { get; set; }

        string AccountScope { get; set; }
    }
}