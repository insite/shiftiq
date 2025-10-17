using System;

namespace InSite.Application.Contacts.Read
{
    public class QMembershipDeletion
    {
        public Guid DeletionIdentifier { get; set; }
        public DateTimeOffset DeletionWhen { get; set; }

        public Guid GroupIdentifier { get; set; }
        public Guid MembershipIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
    }
}
