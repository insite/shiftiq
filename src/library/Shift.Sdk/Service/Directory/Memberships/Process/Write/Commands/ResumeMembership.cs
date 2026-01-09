using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class ResumeMembership : Command
    {
        public Guid User { get; }
        public Guid Group { get; }
        public string Function { get; }
        public DateTimeOffset Effective { get; }

        public ResumeMembership(Guid membership, Guid user, Guid group, string function, DateTimeOffset effective)
        {
            AggregateIdentifier = membership;

            User = user;
            Group = group;
            Function = function;
            Effective = effective;
        }
    }
}
