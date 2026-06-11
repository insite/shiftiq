using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonCreated : Change
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public string FullName { get; set; }

        public PersonCreated(Guid userId, Guid organizationId, string fullName)
        {
            UserId = userId;
            OrganizationId = organizationId;
            FullName = fullName;
        }
    }
}