using System;

using Common.Timeline.Commands;

namespace InSite.Application.Users.Write
{
    public class ArchiveCmdsUser : Command
    {
        public Guid User { get; set; }

        public ArchiveCmdsUser(Guid user)
        {
            User = user;
        }
    }
}
