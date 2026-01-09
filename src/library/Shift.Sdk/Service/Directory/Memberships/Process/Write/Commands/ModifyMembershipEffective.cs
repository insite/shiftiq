using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write
{
    public class ModifyMembershipEffective : Command
    {
        public DateTimeOffset Effective { get; }

        public ModifyMembershipEffective(Guid membership, DateTimeOffset effective)
        {
            AggregateIdentifier = membership;

            Effective = effective;
        }
    }
}
