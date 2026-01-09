using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonSecretRemoved : Change
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }

        public PersonSecretRemoved(Guid userId, Guid organizationId)
        {
            UserId = userId;
            OrganizationId = organizationId;
        }
    }
}