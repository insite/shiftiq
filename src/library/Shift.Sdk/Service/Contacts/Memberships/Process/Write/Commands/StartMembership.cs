using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class StartMembership : Command
    {
        public Guid User { get; }
        public Guid Group { get; }
        public string Function { get; }
        public DateTimeOffset Effective { get; }

        public StartMembership(Guid membership, Guid user, Guid group, string function, DateTimeOffset effective)
        {
            AggregateIdentifier = membership;

            User = user;
            Group = group;
            Function = function;
            Effective = effective;
        }
    }
}
