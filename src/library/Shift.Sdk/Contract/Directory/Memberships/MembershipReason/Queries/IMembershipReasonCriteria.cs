using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMembershipReasonCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? CreatedBy { get; set; }
        Guid? MembershipIdentifier { get; set; }
        Guid? ModifiedBy { get; set; }

        string LastChangeType { get; set; }
        string PersonOccupation { get; set; }
        string ReasonSubtype { get; set; }
        string ReasonType { get; set; }

        DateTimeOffset? Created { get; set; }
        DateTimeOffset? Modified { get; set; }
        DateTimeOffset? ReasonEffective { get; set; }
        DateTimeOffset? ReasonExpiry { get; set; }
    }
}