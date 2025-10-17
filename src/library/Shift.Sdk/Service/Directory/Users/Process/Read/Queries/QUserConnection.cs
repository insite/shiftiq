using System;

namespace InSite.Application.Contacts.Read
{
    public class QUserConnection
    {
        public Guid FromUserIdentifier { get; set; }
        public Guid ToUserIdentifier { get; set; }

        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }
        public bool IsLeader { get; set; }

        public DateTimeOffset Connected { get; set; }

        public virtual QUser ToUser { get; set; }
        public virtual QUser FromUser { get; set; }
    }
}
