using System;

namespace InSite.Domain.Contacts
{
    public class UserConnection
    {
        public Guid ToUser { get; set; }

        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }

        public DateTimeOffset Connected { get; set; }
    }
}
