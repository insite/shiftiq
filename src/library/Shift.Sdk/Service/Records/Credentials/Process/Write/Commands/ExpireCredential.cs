using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class ExpireCredential : Command
    {
        public ExpireCredential(Guid credential, DateTimeOffset expired)
        {
            AggregateIdentifier = credential;
            Expired = expired;
        }

        public DateTimeOffset Expired { get; set; }
    }
}