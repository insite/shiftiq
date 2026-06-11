using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class RevokePersonAccess : Command
    {
        public DateTimeOffset Revoked { get; set; }
        public string RevokedBy { get; set; }

        public RevokePersonAccess(Guid personId, DateTimeOffset revoked, string revokedBy)
        {
            AggregateIdentifier = personId;
            Revoked = revoked;
            RevokedBy = revokedBy;
        }
    }
}
