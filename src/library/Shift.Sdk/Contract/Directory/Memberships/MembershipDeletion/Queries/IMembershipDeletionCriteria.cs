using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMembershipDeletionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GroupIdentifier { get; set; }
        Guid? MembershipIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        DateTimeOffset? DeletionWhen { get; set; }
    }
}