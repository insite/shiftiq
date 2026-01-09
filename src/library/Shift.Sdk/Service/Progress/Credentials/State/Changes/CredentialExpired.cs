using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class CredentialExpired : Change
    {
        public CredentialExpired(Guid user, DateTimeOffset? expired)
        {
            User = user;
            Expired = expired;
        }

        public Guid User { get; set; }
        public DateTimeOffset? Expired { get; set; }
    }

    public class CredentialExpired2 : Change
    {
        public CredentialExpired2(DateTimeOffset? expired)
        {
            Expired = expired;
        }

        public DateTimeOffset? Expired { get; set; }
    }
}