using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchMembershipReasons : Query<IEnumerable<MembershipReasonMatch>>, IMembershipReasonCriteria
    {
        public Guid? CreatedBy { get; set; }
        public Guid? MembershipIdentifier { get; set; }
        public Guid? ModifiedBy { get; set; }

        public string LastChangeType { get; set; }
        public string PersonOccupation { get; set; }
        public string ReasonSubtype { get; set; }
        public string ReasonType { get; set; }

        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public DateTimeOffset? ReasonEffective { get; set; }
        public DateTimeOffset? ReasonExpiry { get; set; }
    }
}