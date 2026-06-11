using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class UnassignUser : Command
    {
        public Guid User { get; set; }
        public string Role { get; set; }

        public UnassignUser(Guid aggregate, Guid user, string role)
        {
            AggregateIdentifier = aggregate;
            User = user;
            Role = role;
        }
    }
}
