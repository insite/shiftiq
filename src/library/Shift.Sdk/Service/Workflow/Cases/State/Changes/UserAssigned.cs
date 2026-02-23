using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class UserAssigned : Change
    {
        public Guid User { get; set; }
        public string Role { get; set; }

        public UserAssigned(Guid user, string role)
        {
            User = user;
            Role = role;
        }
    }
}
