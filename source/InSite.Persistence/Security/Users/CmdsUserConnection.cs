using System;

namespace InSite.Persistence
{
    public class CmdsUserConnection
    {
        public Guid UserIdentifier { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool IsLeader { get; set; }
        public bool IsManager { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsValidator { get; set; }

        public bool CanBeLeader { get; set; }
        public bool CanBeManager { get; set; }
        public bool CanBeSupervisor { get; set; }
        public bool CanBeValidator { get; set; }
    }
}
