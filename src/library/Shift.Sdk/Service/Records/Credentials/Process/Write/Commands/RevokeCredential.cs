using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class RevokeCredential : Command
    {
        public RevokeCredential(Guid credential, DateTimeOffset revoked, string reason, decimal? score)
        {
            AggregateIdentifier = credential;
            Revoked = revoked;
            Reason = reason;
            Score = score;
        }

        public DateTimeOffset Revoked { get; set; }
        public string Reason { get; set; }
        public decimal? Score { get; set; }
    }
}