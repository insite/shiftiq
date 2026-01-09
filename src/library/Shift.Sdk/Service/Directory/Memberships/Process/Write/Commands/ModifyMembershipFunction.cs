using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class ModifyMembershipFunction : Command
    {
        public string Function { get; }

        public ModifyMembershipFunction(Guid membership, string function)
        {
            AggregateIdentifier = membership;

            Function = function;
        }
    }

    public class ModifyMembershipExpiry : Command
    {
        public DateTimeOffset? Expiry { get; }

        public ModifyMembershipExpiry(Guid membership, DateTimeOffset? expiry)
        {
            AggregateIdentifier = membership;

            Expiry = expiry;
        }
    }

    public class ExpireMembership : Command
    {
        public DateTimeOffset Expiry { get; }

        public ExpireMembership(Guid membership, DateTimeOffset expiry)
        {
            AggregateIdentifier = membership;

            Expiry = expiry;
        }
    }
}
