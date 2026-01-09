using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class CredentialRevoked : Change
    {
        public CredentialRevoked(Guid user, DateTimeOffset revoked, string reason)
        {
            User = user;
            Revoked = revoked;
            Reason = reason;
        }

        public Guid User { get; set; }
        public DateTimeOffset Revoked { get; set; }
        public string Reason { get; set; }
    }

    public class CredentialRevoked2 : Change
    {
        public CredentialRevoked2(DateTimeOffset revoked, string reason, decimal? score)
        {
            Revoked = revoked;
            Reason = reason;
            Score = score;
        }

        public DateTimeOffset Revoked { get; set; }
        public string Reason { get; set; }
        public decimal? Score { get; set; }
    }
}