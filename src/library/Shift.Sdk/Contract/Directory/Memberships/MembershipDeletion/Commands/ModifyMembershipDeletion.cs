using System;

namespace Shift.Contract
{
    public class ModifyMembershipDeletion
    {
        public Guid DeletionIdentifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public Guid MembershipIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public DateTimeOffset DeletionWhen { get; set; }
    }
}