using System;

namespace InSite.Persistence
{
    public class UserConnection
    {
        public Boolean IsLeader { get; set; }
        public Boolean IsManager { get; set; }
        public Boolean IsSupervisor { get; set; }
        public Boolean IsValidator { get; set; }
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }
        public DateTimeOffset Connected { get; set; }

        public virtual User FromUser { get; set; }
        public virtual User ToUser { get; set; }
    }
}
