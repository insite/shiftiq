using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Cases.Write
{
    public class AssignUser : Command
    {
        public Guid User { get; set; }
        public string Role { get; set; }

        public AssignUser(Guid aggregate, Guid user, string role)
        {
            AggregateIdentifier = aggregate;
            User = user;
            Role = role;
        }
    }
}
