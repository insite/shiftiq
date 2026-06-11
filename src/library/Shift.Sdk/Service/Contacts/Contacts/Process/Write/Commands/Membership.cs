using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Memberships.Write.Commands
{
    public class JoinMembership : Command
    {
        public Guid Group { get; set; }
        public Guid User { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Joined { get; set; }

        public JoinMembership(Guid membership, Guid group, Guid user, string type, string description, DateTimeOffset? joined)
        {
            AggregateIdentifier = membership;
            Group = group;
            User = user;
            Type = type;
            Description = description;
            Joined = joined;
        }
    }

    public class LeaveMembership : Command
    {
        public string Reason { get; set; }

        public LeaveMembership(Guid membership, string reason)
        {
            AggregateIdentifier = membership;
            Reason = reason;
        }
    }

    public class ChangeMembership : Command
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? Joined { get; set; }

        public ChangeMembership(Guid membership, string type, string description, DateTimeOffset? joined)
        {
            AggregateIdentifier = membership;
            Type = type;
            Description = description;
            Joined = joined;
        }
    }
}
