using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class UnarchiveCmdsUser : Command
    {
        public Guid User { get; set; }
        public Guid Organization { get; set; }
        public Guid Person { get; set; }

        public UnarchiveCmdsUser(Guid user, Guid organization, Guid person)
        {
            User = user;
            Organization = organization;
            Person = person;
        }
    }
}
