using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Contacts
{
    public class PersonAccessRevoked : Change
    {
        public DateTimeOffset Revoked { get; set; }
        public string RevokedBy { get; set; }

        public PersonAccessRevoked(DateTimeOffset revoked, string revokedBy)
        {
            Revoked = revoked;
            RevokedBy = revokedBy;
        }
    }
}
