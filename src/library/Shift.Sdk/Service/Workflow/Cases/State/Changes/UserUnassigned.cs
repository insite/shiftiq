using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Issues
{
    public class UserUnassigned : Change
    {
        public Guid User { get; set; }
        public string Role { get; set; }

        public UserUnassigned(Guid user, string role)
        {
            User = user;
            Role = role;
        }
    }
}
