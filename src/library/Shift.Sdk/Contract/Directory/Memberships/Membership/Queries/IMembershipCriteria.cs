using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMembershipCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? GroupIdentifier { get; set; }
        Guid? ModifiedBy { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string MembershipFunction { get; set; }

        DateTimeOffset? MembershipEffective { get; set; }
        DateTimeOffset? MembershipExpiry { get; set; }
        DateTimeOffset? Modified { get; set; }

        string AccountScope { get; set; }
    }
}