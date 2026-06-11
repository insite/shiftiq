using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Credentials.Write
{
    public class CreateCredential : Command
    {
        public CreateCredential(Guid credential, Guid tenant, Guid achievement, Guid user, DateTimeOffset? assigned)
        {
            AggregateIdentifier = credential;
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