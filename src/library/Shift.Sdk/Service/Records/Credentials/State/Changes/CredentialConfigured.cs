using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Obsolete]
    public class CredentialConfigured : Change
    {
        public CredentialConfigured(Guid user, Expiration expiration, string necessity, string priority)
        {
            User = user;
            Expiration = expiration;
            Necessity = necessity;
            Priority = priority;
        }

        public Guid User { get; set; }
        public Expiration Expiration { get; set; }
        public string Necessity { get; set; }
        public string Priority { get; set; }
    }
}