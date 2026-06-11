using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Records;

namespace InSite.Application.Credentials.Write
{
    public class ChangeCredentialExpiration : Command
    {
        public ChangeCredentialExpiration(Guid credential, Expiration expiration)
        {
            AggregateIdentifier = credential;
            Expiration = expiration;
        }

        public Expiration Expiration { get; set; }
    }
}