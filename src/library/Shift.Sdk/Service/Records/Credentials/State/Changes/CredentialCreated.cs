using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class CredentialCreated : Change
    {
        public CredentialCreated(Guid tenant, Guid achievement, Guid user, DateTimeOffset? assigned)
        {
            Tenant = tenant;
            Achievement = achievement;
            User = user;
            Assigned = assigned;
        }

        public Guid Tenant { get; set; }
        public Guid Achievement { get; set; }
        public Guid User { get; set; }
        public DateTimeOffset? Assigned { get; set; }
    }
}