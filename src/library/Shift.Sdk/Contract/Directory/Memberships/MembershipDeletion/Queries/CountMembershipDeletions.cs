using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountMembershipDeletions : Query<int>, IMembershipDeletionCriteria
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? MembershipIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public DateTimeOffset? DeletionWhen { get; set; }
    }
}