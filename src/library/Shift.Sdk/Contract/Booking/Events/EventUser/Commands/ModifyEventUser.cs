using System;

namespace Shift.Contract
{
    public class ModifyEventUser
    {
        public Guid EventIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public string AttendeeRole { get; set; }

        public DateTimeOffset? Assigned { get; set; }
    }
}